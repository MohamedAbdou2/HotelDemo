using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
