using Application.Dtos;
using Application.Dtos.User;
using Application.Dtos.User.Staff;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto<bool>> Register(RegisterDto dto);
        Task<ResponseDto<bool>> StaffRegister(StaffRegisterDto dto);

        Task<ResponseDto<string>> Login(LoginDto dto);
        Task<ResponseDto<bool>> UpdateUser(Guid Id, UpdateUserDto dto);

        Task<ResponseDto<string>> ForgetPassword(string Email);
        Task<ResponseDto<bool>> ResetPassword(ResetPasswordDto dto);

        Task<ResponseDto<bool>> UpdateRole(UpdateRoleDto dto, Guid AdminId);


    }
}
