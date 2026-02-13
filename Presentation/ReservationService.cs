using Application.Dtos;
using Application.Dtos.Reservation;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Application.Services.ReservationServices
{
    public class ReservationService : IReservationService
    {
        private const int ReservationExpirationMinutes = 10;
        private const int MaxRetryAttempts = 3;
        private const int BaseRetryDelayMilliseconds = 200;
        private const int MaxJitterMilliseconds = 50;

        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ReservationDto> _reservationValidator;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IReadOnlyRepository<RoomOffer> _roomOffersRepository;

        public ReservationService(
            IGenericRepository<Room> roomRepository,
            IGenericRepository<Reservation> reservationRepository,
            IGenericRepository<Payment> paymentRepository,
            IMapper mapper,
            IValidator<ReservationDto> reservationValidator,
            IBackgroundJobService backgroundJobService,
            IReadOnlyRepository<RoomOffer> roomOffersRepository)
        {
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _reservationValidator = reservationValidator;
            _backgroundJobService = backgroundJobService;
            _roomOffersRepository = roomOffersRepository;
        }

        public async Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto)
        {
            var retryPolicy = CreateRetryPolicy();

            return await retryPolicy.ExecuteAsync(async () =>
            {
                var validationResult = await ValidateReservationAsync(reservationDto);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var room = await GetAndValidateRoomAsync(reservationDto.RoomId, reservationDto.CheckInDate, reservationDto.CheckOutDate);
                if (room == null)
                {
                    return ResponseDto<ReservationResponseDto>.Fail(
                        ErrorCode.RoomNotFound,
                        "The room is currently not available for booking.");
                }

                var reservation = await CreateReservationEntityAsync(reservationDto, room);
                await SaveReservationAndUpdateRoomAsync(reservation, room);

                _backgroundJobService.ScheduleReservationCancellation(
                    reservation.Id,
                    TimeSpan.FromMinutes(ReservationExpirationMinutes));

                var response = BuildReservationResponse(reservation, room);
                return ResponseDto<ReservationResponseDto>.Success(
                    response,
                    $"Reservation created successfully! Please complete the payment within {ReservationExpirationMinutes} minutes.");
            });
        }

        public async Task<bool> IsRoomAvailable(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            return !await _reservationRepository.IsExist(r =>
                r.RoomId == roomId &&
                r.ReservationStatusId != ReservationStatusCode.Cancelled &&
                r.ReservationStatusId != ReservationStatusCode.CheckedOut &&
                checkIn < r.CheckOutDate &&
                checkOut > r.CheckInDate);
        }

        public async Task CheckAndCancelReservation(Guid reservationId)
        {
            var reservationQuery = await _reservationRepository.GetbyId(reservationId);
            var reservation = reservationQuery.FirstOrDefault();

            if (reservation == null || reservation.ReservationStatusId != ReservationStatusCode.Pending)
            {
                return;
            }

            await CancelReservationAsync(reservation);
            await ReleaseRoomAsync(reservation.RoomId);
        }

        public async Task<ResponseDto<ReservationResponseDto>> GetReservationById(Guid reservationId)
        {
            var reservationQuery = await _reservationRepository.GetbyId(reservationId);
            var reservation = reservationQuery.FirstOrDefault();

            if (reservation == null)
            {
                return ResponseDto<ReservationResponseDto>.Fail(
                    ErrorCode.NotFound,
                    "Reservation not found.");
            }

            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.Status = reservation.ReservationStatusId.ToString();

            return ResponseDto<ReservationResponseDto>.Success(response);
        }

        #region Private Helper Methods

        private static IAsyncPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<DbUpdateConcurrencyException>()
                .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt =>
                    TimeSpan.FromMilliseconds(BaseRetryDelayMilliseconds * retryAttempt) +
                    TimeSpan.FromMilliseconds(new Random().Next(0, MaxJitterMilliseconds)));
        }

        private async Task<ResponseDto<ReservationResponseDto>?> ValidateReservationAsync(ReservationDto reservationDto)
        {
            var validationResult = _reservationValidator.Validate(reservationDto);
            if (!validationResult.IsValid)
            {
                return ResponseDto<ReservationResponseDto>.ValidationFail(validationResult);
            }

            var isAvailable = await IsRoomAvailable(
                reservationDto.RoomId,
                reservationDto.CheckInDate,
                reservationDto.CheckOutDate);

            if (!isAvailable)
            {
                return ResponseDto<ReservationResponseDto>.Fail(
                    ErrorCode.RoomNotAvailable,
                    "The room is already booked for the selected period.");
            }

            return null;
        }

        private async Task<Room?> GetAndValidateRoomAsync(Guid roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var roomQuery = await _roomRepository.GetbyId(roomId);
            var room = roomQuery.FirstOrDefault();

            if (room == null || !room.IsAvailable)
            {
                return null;
            }

            return room;
        }

        private async Task<Reservation> CreateReservationEntityAsync(ReservationDto reservationDto, Room room)
        {
            var reservation = _mapper.Map<Reservation>(reservationDto);
            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(ReservationExpirationMinutes);
            reservation.ReservationStatusId = ReservationStatusCode.Pending;

            var discount = await CheckAndGetDiscountOfferAsync(reservationDto.RoomId, reservationDto.CheckInDate);
            var numberOfDays = (decimal)(reservationDto.CheckOutDate - reservationDto.CheckInDate).TotalDays;
            
            reservation.Discount = discount;
            reservation.TotalPrice = CalculateTotalPrice(room.PricePerNight, numberOfDays, discount);

            return reservation;
        }

        private static decimal CalculateTotalPrice(decimal pricePerNight, decimal numberOfDays, decimal discountPercentage)
        {
            if (discountPercentage > 0)
            {
                return pricePerNight * (1 - discountPercentage / 100) * numberOfDays;
            }

            return pricePerNight * numberOfDays;
        }

        private async Task SaveReservationAndUpdateRoomAsync(Reservation reservation, Room room)
        {
            await _reservationRepository.Add(reservation);

            room.IsAvailable = false;
            await _roomRepository.UpdateIncludeAsync(room, x => x.IsAvailable);
        }

        private ReservationResponseDto BuildReservationResponse(Reservation reservation, Room room)
        {
            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.RoomNumber = room.RoomNumber;
            response.Status = reservation.ReservationStatusId.ToString();
            return response;
        }

        private async Task<decimal> CheckAndGetDiscountOfferAsync(Guid roomId, DateTime checkInDate)
        {
            var roomOfferQuery = await _roomOffersRepository.GetAll(ro => ro.RoomId == roomId);
            var roomOffer = await roomOfferQuery.Include(x => x.Offer).FirstOrDefaultAsync();

            if (!IsOfferValid(roomOffer, checkInDate))
            {
                return 0;
            }

            return roomOffer.Offer.DiscountPercentage;
        }

        private static bool IsOfferValid(RoomOffer? roomOffer, DateTime checkInDate)
        {
            return roomOffer != null &&
                   roomOffer.Offer.IsActive &&
                   roomOffer.Offer.StartDate <= checkInDate &&
                   roomOffer.Offer.EndDate >= checkInDate;
        }

        private async Task CancelReservationAsync(Reservation reservation)
        {
            reservation.ReservationStatusId = ReservationStatusCode.Cancelled;
            await _reservationRepository.UpdateIncludeAsync(reservation, nameof(Reservation.ReservationStatusId));
        }

        private async Task ReleaseRoomAsync(Guid roomId)
        {
            var roomQuery = await _roomRepository.GetbyId(roomId);
            var room = roomQuery.FirstOrDefault();

            if (room != null)
            {
                room.IsAvailable = true;
                await _roomRepository.Update(room);
            }
        }

        #endregion
    }
}