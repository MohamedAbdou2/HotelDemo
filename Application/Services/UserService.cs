using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Dtos.User;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using HotelDemo.Helper;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Numerics;
using Application.Dtos.User.Staff;
using FluentValidation;
using Application.Validator.UserValidator;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly IMapper mapper;
        private readonly JwtSettings jwtSettings;
        private readonly IGenericRepository<UserOtp> userotprepo;
        private readonly IGenericRepository<Staff> staffRepository;
        private readonly IReadOnlyRepository<Role> roleRepository;
        private readonly IGenericRepository<UserRole> userRoleRepository;
        private readonly IValidator<StaffRegisterDto> _staffRegisterDtoValidator;
        private readonly IValidator<UpdateRoleDto> _updateRoleDtoValidator;
        public UserService(IGenericRepository<User> userRepository,
            IMapper mapper,
            JwtSettings jwtSettings,
            IGenericRepository<UserOtp> userOtpRepo,
            IGenericRepository<Staff> staffRepository,
            IReadOnlyRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IValidator<StaffRegisterDto> staffRegisterDtoValidator,
            IValidator<UpdateRoleDto> updateRoleDtoValidator)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.jwtSettings = jwtSettings;
            this.userotprepo = userOtpRepo;
            this.staffRepository = staffRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            _staffRegisterDtoValidator = staffRegisterDtoValidator;
            _updateRoleDtoValidator = updateRoleDtoValidator;
        }
        public async Task<ResponseDto<bool>> Register(RegisterDto dto)
        {
            var isUserExist = await userRepository.IsExist(x => x.Email == dto.email);
            if (isUserExist)
                return ResponseDto<bool>.Fail(ErrorCode.EmailalreadyExist, "A user with this eamil is already resgistered");

            var user = mapper.Map<User>(dto);
            var result = await userRepository.Add(user);

            var customerRole = await roleRepository.GetAll(x => x.Name == "Customer");
            var customerRoleId = customerRole.FirstOrDefault()?.Id;

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = customerRoleId.Value,
            };

            result = await userRoleRepository.Add(userRole);

            return ResponseDto<bool>.Success(result, "Registration successfull");
        }
        public async Task<ResponseDto<bool>> StaffRegister(StaffRegisterDto dto)
        {
            var validationResult = _staffRegisterDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidaitonFial(validationResult);

            var isUserExist = await userRepository.IsExist(x => x.Email == dto.email);
            if (isUserExist)
                return ResponseDto<bool>.Fail(ErrorCode.EmailalreadyExist, "A user with this eamil is already resgistered");

            var user = mapper.Map<User>(dto);

            var result = await userRepository.Add(user);

            var staff = new Staff
            {
                UserId = user.Id,
                Position = dto.Position,
                HireDate = dto.HireDate,
                TerminationDate = dto.TerminationDate,
            };
            result = await staffRepository.Add(staff);
            var staffRole = await roleRepository.GetAll(x => x.Name == "Staff");
            var staffRoleId = staffRole.FirstOrDefault()?.Id;

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = staffRoleId.Value,
            };

            result = await userRoleRepository.Add(userRole);

            return ResponseDto<bool>.Success(result, "Registration successfull");
        }
        public async Task<ResponseDto<string>> Login(LoginDto dto)
        {
            var userQurable = await userRepository.GetAll(x => x.Email == dto.Email);
            var user = userQurable.FirstOrDefault();
            if (user == null)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "User is either not registered or is deleted");

            if (dto.Email != user.Email && BCrypt.Net.BCrypt.HashPassword(dto.Password) != user.PasswordHash)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "wrong credentials");

            var rolesQurable = await userRoleRepository.GetAll(x => x.UserId == user.Id);
            var roles = rolesQurable.Select(x => x.Role.Name).ToList();

            var token = new GenerateToken(jwtSettings).GenerateJwtToken(user.Id.ToString(), user.Email, roles);

            return ResponseDto<string>.Success(token);
        }

        public async Task<ResponseDto<string>> ForgetPassword(string email)
        {
            var userQurable = await userRepository.GetAll(x => x.Email == email);
            var user = userQurable.FirstOrDefault();

            if (user == null)
                return ResponseDto<string>.Fail(ErrorCode.EmailNotRegistered, "This email is not registered");

            var otp = new UserOtp()
            {
                otp = RandomNumberGenerator.GetInt32(111111, 1_000_000).ToString(),
                UserId = user.Id,
                ExpiresAt = DateTime.Now.AddMinutes(5)

            };
            await new MailSender().SendAsync(email, "Password Reset", $"{otp.otp}");

            return ResponseDto<string>.Success("Check your email");

        }

        public async Task<ResponseDto<bool>> ResetPassword(ResetPasswordDto dto)
        {
            var userOtpQuerable = await userotprepo.GetAll(x => x.otp == dto.otp);
            var useropt = userOtpQuerable.FirstOrDefault();

            if (useropt == null || useropt.ExpiresAt > DateTime.Now)
                return ResponseDto<bool>.Fail(ErrorCode.InvalidOtp, "otp is either Invalid or expired");

            var user = new User
            {
                Id = useropt.UserId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.password)
            };

            await userRepository.UpdateIncludeAsync(user, nameof(User.PasswordHash));

            var otp = new UserOtp
            {
                Id = useropt.Id,
                IsDeleted = true,
            };

            await userRepository.UpdateIncludeAsync(user, nameof(User.IsDeleted));

            return ResponseDto<bool>.Success(true, "Password reset successfull");
        }
        public async Task<ResponseDto<bool>> UpdateRole(UpdateRoleDto dto, Guid adminId)
        {
            var validationResult = _updateRoleDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidaitonFial(validationResult);

            if (!await IsAdmin(adminId))
                    return ResponseDto<bool>.Fail(ErrorCode.BadRequest, "Only admin can change user roles");

                var userQurable = await userRepository.GetAll(x => x.Id == dto.userId);
                var user = userQurable.FirstOrDefault();
                if (user == null)
                    return ResponseDto<bool>.Fail(ErrorCode.UserNotFound, "User not found");

                var roleQurable = await roleRepository.GetAll(x => x.Name == dto.roleName);
                var role = roleQurable.FirstOrDefault();
                if (role == null)
                    return ResponseDto<bool>.Fail(ErrorCode.RoleNotFound, "Role not found");

                var userRoleQurable = await userRoleRepository.GetAll(x => x.UserId == dto.userId);
                var userRole = userRoleQurable.FirstOrDefault();
                if (userRole == null)
                    return ResponseDto<bool>.Fail(ErrorCode.UserRoleNotFound, "User role not found");

                userRole.RoleId = role.Id;
                await userRoleRepository.UpdateIncludeAsync(userRole, nameof(UserRole.RoleId));
                return ResponseDto<bool>.Success(true, "User role updated successfully");
            
        }
        public async Task<bool> IsAdmin(Guid userId)
        {
            var userRoleQurable = await userRoleRepository.GetAll(x => x.UserId == userId);
            var userRole = userRoleQurable.FirstOrDefault();
            if (userRole == null)
                return false;
            var roleQurable = await roleRepository.GetAll(x => x.Id == userRole.RoleId);
            var role = roleQurable.FirstOrDefault();
            if (role == null)
                return false;
            return role.Name == "Admin";
        }

        /*      public async Task<ResponseDto<bool>> ChangePassword(ChangePasswordDto dto, Guid userId)
              {
                  var userQurable = await userRepository.GetAll(x => x.Id == userId);
                  var user = userQurable.FirstOrDefault();
                  if (user == null)
                      return ResponseDto<bool>.Fail(ErrorCode.UserNotFound, "User not found");
                  if (!BCrypt.Net.BCrypt.Verify(dto.currentPassword, user.PasswordHash))
                      return ResponseDto<bool>.Fail(ErrorCode.InvalidCurrentPassword, "Current password is incorrect");
                  user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.newPassword);
                  await userRepository.UpdateIncludeAsync(user, nameof(User.PasswordHash));
                  return ResponseDto<bool>.Success(true, "Password changed successfully");
              }*/
    }

}
