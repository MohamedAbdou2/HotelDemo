using Application.Dtos.Reservation;
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
        private readonly IReservationServices _reservationService;
        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;

        public ReservationController(
            IReservationServices reservationService,
            LinkGenerator linkGenerator,
            IMapper mapper)
        {
            _reservationService = reservationService;
            _linkGenerator = linkGenerator;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new reservation
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseViewModel<ReservationResponseViewModel>), 201)]
        [ProducesResponseType(typeof(ResponseViewModel<ReservationResponseViewModel>), 400)]
        public async Task<ResponseViewModel<ReservationResponseViewModel>> CreateReservation(
            [FromBody] CreateReservationViewModel model)
        {
            // 1. Map ViewModel to DTO (same as UserController)
            var dto = _mapper.Map<ReservationDto>(model);

            // 2. Call service (same as UserController)
            var serviceResult = await _reservationService.CreateReservation(dto);

            if (!serviceResult.IsSuccess)
                return ResponseViewModel<ReservationResponseViewModel>.Fail(
                    serviceResult.ErrorCode,
                    serviceResult.Message);

            // 3. Generate URLs with LinkGenerator ✨
            var locationUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: nameof(GetReservationById),
                values: new { id = serviceResult.Data!.Id });

            serviceResult.Data.PaymentUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: "InitiatePayment",
                controller: "Payment",
                values: new { reservationId = serviceResult.Data.Id }) ?? string.Empty;

            // 4. Map DTO to ViewModel (same as UserController)
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

        /// <summary>
        /// Get reservation by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseViewModel<ReservationResponseViewModel>), 200)]
        [ProducesResponseType(typeof(ResponseViewModel<ReservationResponseViewModel>), 404)]
        public async Task<ResponseViewModel<ReservationResponseViewModel>> GetReservationById(Guid id)
        {
            // 1. Call service (same as UserController)
            var serviceResult = await _reservationService.GetReservationById(id);

            if (!serviceResult.IsSuccess)
                return ResponseViewModel<ReservationResponseViewModel>.Fail(
                    serviceResult.ErrorCode,
                    serviceResult.Message);

            // 2. Generate URLs with LinkGenerator ✨
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
