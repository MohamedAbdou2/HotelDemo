using Application.Dtos.User.Staff;
using Application.Helper;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.User.Staff;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly CurrentUser _currentUser;

        public AdminController(IUserService userService,
            IMapper mapper,
            CurrentUser currentUser)
        {
            _userService = userService;
            _mapper = mapper;
            this._currentUser = currentUser;
        }
        /* public ActionResult<ResultViewModel> GetAllUsers()
         {
             var result = _userService.GetAllUsers();
             return Ok(result);
         }
 */
      //  [Authorize/*(Roles ="admin")*/]
        [HttpPost("staff/register")]
        public async Task<ActionResult<ResponseViewModel<bool>>> RegisterStaff([FromBody]StaffRegisterViewModel model)
        {
            var userId = _currentUser.GetUserId();

            if (userId == Guid.Empty)
                return ResponseViewModel<bool>.Fail(ErrorCode.UserNotFound, "Can not Find User");

            var dto = _mapper.Map<StaffRegisterDto>(model);

            var result = await _userService.StaffRegister(dto);

            return result.Data ? 
                ResponseViewModel<bool>.Success(result.Data , "staff register successfully")  
                : ResponseViewModel<bool>.Fail(result.ErrorCode, "Error Aqure When regiter staff" + "\n" +  result.Message);
        }
    }
}
