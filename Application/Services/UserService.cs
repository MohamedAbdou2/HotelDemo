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
        private readonly IReadOnlyRepository<Role> roleRepository;
        private readonly IGenericRepository<UserRole> userRoleRepository;
        private readonly IGenericRepository<Customer> customerRepository;
        private readonly IValidator<RegisterDto> registerDtoValidator;
        private readonly IValidator<LoginDto> loginDtoValidator;
        private readonly IValidator<ResetPasswordDto> resetPasswordDtoValidator;

        public UserService(IGenericRepository<User> userRepository, 
            IMapper mapper, 
            JwtSettings jwtSettings,
            IGenericRepository<UserOtp> userOtpRepo,
            IReadOnlyRepository<Role> roleRepository, 
            IGenericRepository<UserRole> userRoleRepository,
            IGenericRepository<Customer> customerRepository,
            IValidator<RegisterDto> registerDtoValidator,
            IValidator<LoginDto> loginDtoValidator,
            IValidator<ResetPasswordDto> resetPasswordDtoValidator)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.jwtSettings = jwtSettings;
            this.userotprepo = userOtpRepo;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
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
    }

}
