using Application.Dtos.User;
using Application.Interfaces;
using AutoMapper;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.User;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UserController(IUserService userService , IMapper mapper)
        {
            this.userService = userService;
            this.mapper = mapper;
        }

        [HttpPost("register")]

        public async Task<ResponseViewModel<bool>> Register(RegisterViewModel model)
        {
            var dto = mapper.Map<RegisterDto>(model);
           var resultdto = await userService.Register(dto);

            return resultdto.IsSuccess ? ResponseViewModel<bool>.Success(resultdto.Data, resultdto.Message)
                : ResponseViewModel<bool>.Fail(resultdto.ErrorCode,resultdto.Message);
        }

        [HttpPost("login")]

        public async Task<ResponseViewModel<string>> Login(LoginViewModel model)
        {
            var dto = mapper.Map<LoginDto>(model);
            var resultdto = await userService.Login(dto);

            return resultdto.IsSuccess ? ResponseViewModel<string>.Success(resultdto.Data ?? string.Empty, resultdto.Message)
                : ResponseViewModel<string>.Fail(resultdto.ErrorCode, resultdto.Message);
        }

        [Authorize]
        [HttpPut("update-user/{Id}")]
        public async Task<ResponseViewModel<bool>> UpdateUser([FromRoute]Guid Id, [FromBody]UpdateUserViewModel model)
        {
            var dto = mapper.Map<UpdateUserDto>(model);
            var resultdto = await userService.UpdateUser(Id,dto);

            return resultdto.IsSuccess ? ResponseViewModel<bool>.Success(resultdto.Data, resultdto.Message)
                : ResponseViewModel<bool>.Fail(resultdto.ErrorCode, resultdto.Message);
        }

        [Authorize]
        [HttpPost("forgetPassword")]
        public async Task<ResponseViewModel<string>> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                ResponseViewModel<string>.Fail(null, "email is required");

            var resultdto = await userService.ForgetPassword(email);

            return resultdto.IsSuccess ? ResponseViewModel<string>.Success(resultdto.Data ?? string.Empty, resultdto.Message)
                : ResponseViewModel<string>.Fail(resultdto.ErrorCode, resultdto.Message);
        }

        [Authorize] 
        [HttpPost("resetPassword")]
        public async Task<ResponseViewModel<bool>> ResetPassword(ResetPasswordViewModel model)
        {
            var dto = mapper.Map<ResetPasswordDto>(model);
            var resultdto = await userService.ResetPassword(dto);

            return resultdto.IsSuccess ? ResponseViewModel<bool>.Success(resultdto.Data, resultdto.Message)
                : ResponseViewModel<bool>.Fail(resultdto.ErrorCode, resultdto.Message);
        }
    }
}
