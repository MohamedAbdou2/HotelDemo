using Application.Dtos;
using Application.Dtos.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto<bool>> Register(RegisterDto dto);
        Task<ResponseDto<string>> Login(LoginDto dto);
        Task<ResponseDto<string>> ForgetPassword(string Email);
        Task<ResponseDto<bool>> ResetPassword(ResetPasswordDto dto);


    }
}
