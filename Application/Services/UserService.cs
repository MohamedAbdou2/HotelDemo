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

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly IMapper mapper;
        private readonly JwtSettings jwtSettings;
        private readonly IGenericRepository<UserOtp> userotprepo;
        private readonly IReadOnlyRepository<Role> roleRepository;
        private readonly IGenericRepository<UserRole> userRoleRepository;

        public UserService(IGenericRepository<User> userRepository, 
            IMapper mapper, 
            JwtSettings jwtSettings,
            IGenericRepository<UserOtp> userOtpRepo,
            IReadOnlyRepository<Role> roleRepository, 
            IGenericRepository<UserRole> userRoleRepository)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.jwtSettings = jwtSettings;
            this.userotprepo = userOtpRepo;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
        }
        public async Task<ResponseDto<bool>> Register(RegisterDto dto)
        {
            var isUserExist = await userRepository.IsExist(x=>x.Email == dto.email);
            if (isUserExist)
                return ResponseDto<bool>.Fail(ErrorCode.EmailalreadyExist, "A user with this eamil is already resgistered");

            var user = mapper.Map<User>(dto);
            var result = await userRepository.Add(user);

           var customerRole =await roleRepository.GetAll(x => x.Name == "Customer");
            var customerRoleId = customerRole.FirstOrDefault()?.Id;

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId =  customerRoleId.Value,
            };
            
          result =   await userRoleRepository.Add(userRole);

            return ResponseDto<bool>.Success(result, "Registration successfull");
        }
        public async Task<ResponseDto<string>> Login(LoginDto dto)
        {
            var userQurable = await userRepository.GetAll(x=>x.Email==dto.Email);
            var user = userQurable.FirstOrDefault();
            if (user == null)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "User is either not registered or is deleted");

            if (dto.Email != user.Email && BCrypt.Net.BCrypt.HashPassword(dto.Password) != user.PasswordHash)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "wrong credentials");

            var rolesQurable =await userRoleRepository.GetAll(x=>x.UserId==user.Id);
            var roles = rolesQurable.Select(x=>x.Role.Name).ToList();

            var token = new GenerateToken(jwtSettings).GenerateJwtToken(user.Id.ToString(), user.Email, roles);

            return ResponseDto<string>.Success(token);
        }

        public async Task<ResponseDto<string>> ForgetPassword(string email)
        {
            var userQurable = await userRepository.GetAll(x => x.Email ==email);
            var user = userQurable.FirstOrDefault();

            if (user==null)
                return  ResponseDto<string>.Fail(ErrorCode.EmailNotRegistered, "This email is not registered");

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
                Id= useropt.Id,
                IsDeleted = true,
            };

            await userRepository.UpdateIncludeAsync(user, nameof(User.IsDeleted));

            return ResponseDto<bool>.Success(true, "Password reset successfull");
        }
    }

}
