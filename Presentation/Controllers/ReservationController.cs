using Application.Dtos.Reservation;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Presentation.ViewModels.Reservation;

namespace Presentation.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;
        private readonly CurrentUser _currentUser;

        public ReservationController(
            IReservationService reservationService,
            LinkGenerator linkGenerator,
            IMapper mapper,
            CurrentUser currentUser)
        {
            _reservationService = reservationService;
            _linkGenerator = linkGenerator;
            _mapper = mapper;
            _currentUser = currentUser;
        }


        [HttpPost]
        public async Task<ResponseViewModel<ReservationResponseViewModel>> CreateReservation(
            [FromBody] CreateReservationViewModel model)
        {
            var dto = _mapper.Map<ReservationDto>(model);
            var userId = _currentUser.GetUserId().Value;
            var currentUserRole = _currentUser.GetUserRole();
            if (currentUserRole == "Staff" || currentUserRole == "Admin")
            {
                if(model.CustomerId == null)
                {
                    return ResponseViewModel<ReservationResponseViewModel>.Fail(
                        ErrorCode.ValidationError,
                        "CustomerId is required for Staff and Admin users.");
                }
            } 
            else
            {
                dto.CustomerId = _currentUser.GetCustomerId(userId);
            }
            dto.CreatedById = userId;
            var serviceResult = await _reservationService.CreateReservation(dto);

            if (!serviceResult.IsSuccess)
                return ResponseViewModel<ReservationResponseViewModel>.Fail(
                    serviceResult.ErrorCode,
                    serviceResult.Message);

            var locationUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetReservationById),
                values: new { id = serviceResult.Data!.Id });

            serviceResult.Data.PaymentUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: "InitiatePayment",
                controller: "Payment",
                values: new { reservationId = serviceResult.Data.Id }) ?? string.Empty;

            var viewModel = _mapper.Map<ReservationResponseViewModel>(serviceResult.Data);
            viewModel.GetDetailsUrl = locationUrl ?? string.Empty;

            // 5. Set REST headers
            Response.Headers.Location = locationUrl;
            Response.StatusCode = StatusCodes.Status201Created;

            // 6. Return ResponseViewModel (same as UserController)
            return ResponseViewModel<ReservationResponseViewModel>.Success(
                viewModel,
                serviceResult.Message);
        }

  
        [HttpGet("{id}")]
        public async Task<ResponseViewModel<ReservationResponseViewModel>> GetReservationById(Guid id)
        {
            var serviceResult = await _reservationService.GetReservationById(id);

            if (!serviceResult.IsSuccess)
                return ResponseViewModel<ReservationResponseViewModel>.Fail(
                    serviceResult.ErrorCode,
                    serviceResult.Message);

     

            var viewModel = _mapper.Map<ReservationResponseViewModel>(serviceResult.Data);
        

            // 4. Return ResponseViewModel (same as UserController)
            return ResponseViewModel<ReservationResponseViewModel>.Success(
                viewModel,
                serviceResult.Message);
        }
    }
}
