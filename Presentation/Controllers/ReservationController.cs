using Application.Dtos.Reservation;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Presentation.ViewModels.Reservation;

namespace Presentation.Controllers
{
    /// <summary>
    /// Reservation management controller
    /// Pattern: Same as UserController (ViewModel → DTO → Service → DTO → ViewModel)
    /// </summary>
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
            dto.CreatedById = _currentUser.GetUserId().Value; 
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
            // 1. Call service (same as UserController)
            var serviceResult = await _reservationService.GetReservationById(id);

            if (!serviceResult.IsSuccess)
                return ResponseViewModel<ReservationResponseViewModel>.Fail(
                    serviceResult.ErrorCode,
                    serviceResult.Message);

            // 2. Generate URLs with LinkGenerator 
            serviceResult.Data!.PaymentUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: "InitiatePayment",
                controller: "Payment",
                values: new { reservationId = serviceResult.Data.Id }) ?? string.Empty;

            // 3. Map DTO to ViewModel (same as UserController)
            var viewModel = _mapper.Map<ReservationResponseViewModel>(serviceResult.Data);
            viewModel.GetDetailsUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetReservationById),
                values: new { id = serviceResult.Data.Id }) ?? string.Empty;

            // 4. Return ResponseViewModel (same as UserController)
            return ResponseViewModel<ReservationResponseViewModel>.Success(
                viewModel,
                serviceResult.Message);
        }
    }
}
