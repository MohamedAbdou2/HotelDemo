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
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Org.BouncyCastle.Bcpg.OpenPgp;


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
        private readonly IGenericRepository<Customer> customerRepository;
        private readonly IValidator<StaffRegisterDto> _staffRegisterDtoValidator;
        private readonly IValidator<UpdateRoleDto> _updateRoleDtoValidator;
        private readonly IValidator<RegisterDto> registerDtoValidator;
        private readonly IValidator<LoginDto> loginDtoValidator;
        private readonly IValidator<ResetPasswordDto> resetPasswordDtoValidator;

        public UserService(IGenericRepository<User> userRepository, 
            IMapper mapper, 
            JwtSettings jwtSettings,
            IGenericRepository<UserOtp> userOtpRepo,
            IGenericRepository<Staff> staffRepository,
            IReadOnlyRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IValidator<StaffRegisterDto> staffRegisterDtoValidator,
            IValidator<UpdateRoleDto> updateRoleDtoValidator,
            IGenericRepository<Customer> customerRepository,
            IValidator<RegisterDto> registerDtoValidator,
            IValidator<LoginDto> loginDtoValidator,
            IValidator<ResetPasswordDto> resetPasswordDtoValidator)
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
            this.customerRepository = customerRepository;
            this.registerDtoValidator = registerDtoValidator;
            this.loginDtoValidator = loginDtoValidator;
            this.resetPasswordDtoValidator = resetPasswordDtoValidator;
        }
  
        public async Task<ResponseDto<bool>> Register(RegisterDto dto)
        {
            var validator = registerDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidaitonFail(validator);

            if (await CheckByEmail(dto.email))
               return ResponseDto<bool>.Fail(ErrorCode.EmailNotRegistered, "This email is already registered");

            var user = mapper.Map<User>(dto);
            var result = await userRepository.Add(user);

           var customerRoleQuerable =await roleRepository.GetAll(x => x.Name == "Customer");
            var customerRole = await customerRoleQuerable.FirstOrDefaultAsync();

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id,
            };
            
           result =   await userRoleRepository.Add(userRole);
            var customer = new Customer
            {
                UserId = user.Id,
            };
          result = await customerRepository.Add(customer);   

           return  ResponseDto<bool>.Success(result,"Registration Successfull");
        }
        public async Task<ResponseDto<bool>> StaffRegister(StaffRegisterDto dto)
        {
            var validationResult = _staffRegisterDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidaitonFail(validationResult);

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
            var validator = loginDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<string>.ValidaitonFail(validator);

            var user = await GetUserbyEmail(dto.Email);
            if (user==null)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "User is either not registered or is deleted");

            if (dto.Email != user.Email && BCrypt.Net.BCrypt.HashPassword(dto.Password) != user.PasswordHash)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "wrong credentials");

            var rolesQurable =await userRoleRepository.GetAll(x=>x.UserId==user.Id);
            var roles = await rolesQurable.Select(x=>x.Role.Name).ToListAsync();

            var token = new GenerateToken(jwtSettings).GenerateJwtToken(user.Id.ToString(), user.Email, roles);

            return ResponseDto<string>.Success(token);
        }

        public async Task<ResponseDto<string>> ForgetPassword(string email)
        {   

            var user = await GetUserbyEmail(email);
            if (user==null)
                return ResponseDto<string>.Fail(ErrorCode.EmailNotRegistered, "This user is either not registered or deleted");

            var otp = new UserOtp()
            {
                otp = RandomNumberGenerator.GetInt32(111111, 1_000_000).ToString(),
                UserId = user.Id,
                ExpiresAt = DateTime.Now.AddMinutes(5)

            };
            await  MailSender.SendAsync(email, "Password Reset", $"{otp.otp}");

            return ResponseDto<string>.Success("Check your email");

        }

        public async Task<ResponseDto<bool>> ResetPassword(ResetPasswordDto dto)
        {
            var validator = resetPasswordDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidaitonFail(validator);

            var usetOtp = await ValidateOtp(dto.otp);
            if (usetOtp==null)
             return ResponseDto<bool>.Fail(ErrorCode.InvalidOtp, "otp is either Invalid or expired");

            var user = new User
            {
                Id = usetOtp.UserId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.password)
            };

            await userRepository.UpdateIncludeAsync(user, nameof(User.PasswordHash));

            var otp = new UserOtp
            {
                Id= usetOtp.Id,
                IsDeleted = true,
            };

           var result = await userRepository.UpdateIncludeAsync(user, nameof(User.IsDeleted));

            return ResponseDto<bool>.Success(result, "Password reset successfull");
        }

        private async Task<bool> CheckByEmail(string email)
        {
            var isUserExist = await userRepository.IsExist(x => x.Email == email);
            return isUserExist;
        }

        private async Task<User> GetUserbyEmail(string email)
        {

            var userQurable = await userRepository.GetAll(x => x.Email == email);
            var user = await userQurable.FirstOrDefaultAsync();

            return user;
        }

        private async Task<UserOtp> ValidateOtp(string otp  )
        {
            
            var userOtpQuerable = await userotprepo.GetAll(x => x.otp ==otp);
            var userOtp =await userOtpQuerable.FirstOrDefaultAsync();

            if (userOtp == null || userOtp.ExpiresAt > DateTime.Now)
                return null;

            return userOtp;
        }
        public async Task<ResponseDto<bool>> UpdateRole(UpdateRoleDto dto, Guid adminId)
        {
            var validationResult = _updateRoleDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidaitonFail(validationResult);

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
