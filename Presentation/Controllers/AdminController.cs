using Application.Services;
using AutoMapper;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public AdminController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        /*       public ActionResult<ResultViewModel> GetAllUsers()
               {
                   var result = _userService.GetAllUsers();
                   return Ok(result);
               }*/


        /* public ActionResult<ResponseViewModel<bool>> RegisterStaff()
         {
             var result = _userService.StaffRegister();
             return Ok(result);
         }*/
    }
}
