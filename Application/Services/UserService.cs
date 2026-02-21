using System.Security.Cryptography;
using Application.Dtos;
using Application.Dtos.User;
using Application.Dtos.User.Staff;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;
using HotelDemo.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> userRepository;
        private readonly IMapper mapper;
        private readonly IOptions<JwtSettings> jwtSettings;
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
        private readonly IValidator<UpdateUserDto> updateUserDtoValidator;
        private readonly IValidator<ChangePasswordDto> _changePasswordDtoValidator;
        private readonly CurrentUser currentUser;

        public UserService(IGenericRepository<User> userRepository,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings,
            IGenericRepository<UserOtp> userOtpRepo,
            IGenericRepository<Staff> staffRepository,
            IReadOnlyRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IValidator<StaffRegisterDto> staffRegisterDtoValidator,
            IValidator<UpdateRoleDto> updateRoleDtoValidator,
            IGenericRepository<Customer> customerRepository,
            IValidator<RegisterDto> registerDtoValidator,
            IValidator<LoginDto> loginDtoValidator,
            IValidator<ResetPasswordDto> resetPasswordDtoValidator,
            IValidator<UpdateUserDto> UpdateUserDtoValidator,
            IValidator<ChangePasswordDto> changePasswordDtoValidator,
             CurrentUser currentUser)
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
            updateUserDtoValidator = UpdateUserDtoValidator;
            _changePasswordDtoValidator = changePasswordDtoValidator;
            this.currentUser = currentUser;
        }

        public async Task<ResponseDto<bool>> Register(RegisterDto dto)
        {
            var validator = registerDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidationFail(validator);

            var validateResutl = await RegisterValidat(dto);
            if (validateResutl is not null)
            {
                return validateResutl;
            }

            var user = mapper.Map<User>(dto);
            var result = await userRepository.Add(user);
            var customerRole =await roleRepository.GetAll(x => x.Name == "Customer").FirstOrDefaultAsync();

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id,
            };

            result = await userRoleRepository.Add(userRole);
            var customer = new Customer
            {
                UserId = user.Id,
            };
          result = await customerRepository.Add(customer);   

           return  ResponseDto<bool>.Success(result,"Registration Successfull");
        }

        public async Task<ResponseDto<bool>> UpdateUser(Guid userId , UpdateUserDto dto)
        {
            if (!await userRepository.IsExist(x=>x.Id==userId))
                return ResponseDto<bool>.Fail(ErrorCode.UserNotFound, "There is no user with this Id");

            var validator = updateUserDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidationFail(validator);

            var newUser = new User
            {
                Id = userId,
            };
            mapper.Map(dto, newUser);

            var modifiedprops = typeof(UpdateUserDto)
                .GetProperties()
                .Where(p => p.GetValue(dto) != null)                       
                .Select(p => p.Name)
                .ToArray();

            var result = await userRepository.UpdateIncludeAsync(newUser , modifiedprops);

            return result? ResponseDto<bool>.Success(result,"User Updated Successfully")
                : ResponseDto<bool>.Fail(ErrorCode.FaildedToUpdateUser, "User Update failed");
        }

        public async Task<ResponseDto<bool>> StaffRegister(StaffRegisterDto dto)
        {
            var validationResult = _staffRegisterDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidationFail(validationResult);

            var validateResutl = await RegisterValidat(dto);
            if (validateResutl is not null)
            {
                return validateResutl;
            }

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

            var staffRoleId = roleRepository.GetAll(x => x.Name == "Staff").FirstOrDefault()?.Id;

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = staffRoleId.Value,
            };

            result = await userRoleRepository.Add(userRole);

            return ResponseDto<bool>.Success(result, "Registration successfull");
        }

        private async Task<ResponseDto<bool>>? RegisterValidat(RegisterDto dto ) 
        {

            var isUserExist = await userRepository.IsExist(x => x.Email == dto.email);
            if (isUserExist)
                return ResponseDto<bool>.Fail(ErrorCode.EmailalreadyExist, "A user with this eamil is already resgistered");

            var isUserNameExist = await userRepository.IsExist(x => x.Username == dto.userName);
            if (isUserNameExist)
                return ResponseDto<bool>.Fail(ErrorCode.EmailalreadyExist, "A user with this User Name is already resgistered");

            var isPhoneNumberExist = await userRepository.IsExist(x => x.PhoneNumber == dto.phoneNumber);
            if (isPhoneNumberExist)
                return ResponseDto<bool>.Fail(ErrorCode.EmailalreadyExist, "A user with this phone number is already resgistered");

            return null;
        }
        public async Task<ResponseDto<string>> Login(LoginDto dto)
        {
            var validator = loginDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<string>.ValidationFail(validator);

            var user = await GetUserbyEmail(dto.Email);
            if (user == null)
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "User is either not registered or is deleted");

            if (dto.Email != user.Email || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return ResponseDto<string>.Fail(ErrorCode.UserNotFound, "wrong credentials");

            var roles = await userRoleRepository.GetAll(x => x.UserId == user.Id).Select(x => x.Role.Name).ToListAsync();

            var token = new GenerateToken(jwtSettings).GenerateJwtToken(user.Id.ToString(), user.Email, roles);

            return ResponseDto<string>.Success(token);
        }
        public async Task<ResponseDto<string>> ForgetPassword(string email)
        {

            var user = await GetUserbyEmail(email);
            if (user == null)
                return ResponseDto<string>.Fail(ErrorCode.EmailNotRegistered, "This user is either not registered or deleted");

            var otp = new UserOtp()
            {
                otp = RandomNumberGenerator.GetInt32(111111, 1_000_000).ToString(),
                UserId = user.Id,
                ExpiresAt = DateTime.Now.AddMinutes(5)

            };
            await userotprepo.Add(otp);
            await MailSender.SendAsync(email, "Password Reset", $"{otp.otp}");

            return ResponseDto<string>.Success("Check your email");

        }

        public async Task<ResponseDto<bool>> ResetPassword(ResetPasswordDto dto)
        {
            var validator = resetPasswordDtoValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseDto<bool>.ValidationFail(validator);

            var usetOtp = await ValidateOtp(dto.otp);
            if (usetOtp == null)
                return ResponseDto<bool>.Fail(ErrorCode.InvalidOtp, "otp is either Invalid or expired");

            var user = new User
            {
                Id = usetOtp.UserId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.password)
            };
            //if you choose to use string please remove the other update method and update here as well
            await userRepository.UpdateIncludeAsync(user, x=>x.UserRoles);

            var otp = new UserOtp
            {
                Id = usetOtp.Id,
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
            var user = await userRepository.GetAll(x => x.Email == email).FirstOrDefaultAsync();

            return user;
        }

        private async Task<UserOtp> ValidateOtp(string otp)
        {
            var userOtp = await userotprepo.GetAll(x => x.otp == otp).FirstOrDefaultAsync();

            if (userOtp == null || userOtp.ExpiresAt < DateTime.Now)
                return null;

            return userOtp;
        }
        public async Task<ResponseDto<bool>> UpdateRole(UpdateRoleDto dto, Guid adminId)
        {
           
            var validationResult = _updateRoleDtoValidator.Validate(dto);
            if (!validationResult.IsValid) return ResponseDto<bool>.ValidationFail(validationResult);

          
            if (!await IsAdmin(adminId))
                return ResponseDto<bool>.Fail(ErrorCode.BadRequest, "Only admin can change user roles");

            
            var (userRole, roleId, error) = await GetAndValidateUserRole(dto);
            if (error != null) return error;

          
            await HandleRoleTransition(dto.userId, userRole.RoleId, roleId);

          
            userRole.RoleId = roleId;
            var result = await userRoleRepository.UpdateIncludeAsync(userRole, nameof(UserRole.RoleId));

            return result ? ResponseDto<bool>.Success(true, "Role Updated Successfully")
                          : ResponseDto<bool>.Fail(ErrorCode.FailedToUpdateUserRole, "Failed to update user role");
        }

        private async Task<(UserRole userRole, UserRoleCode targetRoleId, ResponseDto<bool> error)> GetAndValidateUserRole(UpdateRoleDto dto)
        {
            var role =await roleRepository.GetAll(x => x.Name == dto.roleName).FirstOrDefaultAsync();
            if (role == null)
                return (null, default, ResponseDto<bool>.Fail(ErrorCode.RoleNotFound, "الدور غير موجود."));

            var userRole = await userRoleRepository.GetAll(x => x.UserId == dto.userId).FirstOrDefaultAsync();
            if (userRole == null)
                return (null, default, ResponseDto<bool>.Fail(ErrorCode.UserRoleNotFound, "علاقة الدور للمستخدم غير موجودة."));

            
            if (userRole.RoleId == role.Id)
                return (null, default, ResponseDto<bool>.Fail(ErrorCode.UserAlreadyHaveThisRole, "المستخدم يمتلك هذا الدور بالفعل."));

            return (userRole, role.Id, null);
        }
        private async Task HandleRoleTransition(Guid userId, UserRoleCode oldRole, UserRoleCode newRole)
        {
           
            if (newRole == UserRoleCode.Staff)
            {
                var exists = await staffRepository.IsExist(x => x.UserId == userId);
                if (!exists)
                    await staffRepository.Add(new Staff { UserId = userId, HireDate = DateTime.Now });
            }
            else if (oldRole == UserRoleCode.Staff)
            {
                var staffRecord = await staffRepository.GetAll(x => x.UserId == userId).FirstOrDefaultAsync();
                if (staffRecord != null)
                    await staffRepository.Delete(staffRecord.Id);
            }
        }

        public async Task<bool> IsAdmin(Guid userId)
        {
            var userRole = userRoleRepository.GetAll(x => x.UserId == userId).FirstOrDefault();
            if (userRole == null)
                return false;
            var role = roleRepository.GetAll(x => x.Id == userRole.RoleId).FirstOrDefault();
            if (role == null)
                return false;
            return role.Name == "Admin";
        }

        public async Task<ResponseDto<bool>> ChangePassword(ChangePasswordDto dto, Guid userId)
        {
            var validationResult = _changePasswordDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidationFail(validationResult);

            var user = userRepository.GetAll(x => x.Id == userId).FirstOrDefault();
            if (user == null)
                return ResponseDto<bool>.Fail(ErrorCode.UserNotFound, "User not found");

            //if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            //    return ResponseDto<bool>.Fail(ErrorCode.InvalidCurrentPassword, "Current password is incorrect");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
   
            await userRepository.UpdateIncludeAsync(user, x => x.PasswordHash);
            return ResponseDto<bool>.Success(true, "Password changed successfully");
        }

        public async Task<ResponseDto<UserDto>> GetUserbyId()
        {
            var userId = currentUser.GetUserId();

            var userDto = await userRepository.GetAll(x => x.Id == userId).ProjectTo<UserDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();

                return userDto != null ? ResponseDto<UserDto>.Success(userDto)
                :ResponseDto<UserDto>.Fail(ErrorCode.UserNotFound,"User not found");

        }


    }

}
