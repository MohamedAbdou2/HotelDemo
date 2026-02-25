using Application.Dtos;
using Application.Dtos.Reservation;
using Application.Dtos.Room;
using Application.Interfaces;
using Application.Services.OfferServices;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Application.Services.ReservationServices
{
    public class RerservationService : IReservationService
    {
        private readonly IRoomService _roomServices;
        private readonly IGenericRepository<Reservation> _reservationRepository;

        private readonly IMapper _mapper;
        private readonly IValidator<ReservationDto> _reservationValidator;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IRoomOfferService _RoomOffersServices;

        private const int ReservationExpirationMinutes = 10;
        private const int MaxRetryAttempts = 3;
        private const int BaseRetryDelayMilliseconds = 200;
        private const int MaxRetryJitterMilliseconds = 50;

        public RerservationService
            (
            IGenericRepository<Reservation> reservationRepository,
            IMapper mapper,
            IValidator<ReservationDto> reservationValidator,
            IBackgroundJobService backgroundJobService,
            IRoomService roomServices,
            IRoomOfferService roomOffersServices
            )
        {

            _reservationRepository = reservationRepository;

            _mapper = mapper;
            _reservationValidator = reservationValidator;
            _backgroundJobService = backgroundJobService;
            _roomServices = roomServices;
            _RoomOffersServices = roomOffersServices;
        }

        public async Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto)
        {
            var retryPolicy = CreateRetryPolicy();
            return await retryPolicy.ExecuteAsync(async () => await CreateReservationWithRetry(reservationDto));
        }

        private async Task<ResponseDto<ReservationResponseDto>> CreateReservationWithRetry(ReservationDto reservationDto)
        {
            var validationResult = _reservationValidator.Validate(reservationDto);
            if (!validationResult.IsValid)
                return ResponseDto<ReservationResponseDto>.ValidationFail(validationResult);
            if (reservationDto.CustomerId is null)
            {
                return ResponseDto<ReservationResponseDto>.Fail(
                    ErrorCode.ValidationError,
                    "Customer ID is required");
            }

            var (room, roomError) = await ValidateAndGetRoom(reservationDto.RoomId);
            if (roomError != null)
                return roomError;

            var availabilityCheck = await ValidateRoomReservationDate(reservationDto);
            if (availabilityCheck != null)
                return availabilityCheck;

            var reservation = await CreateReservationEntity(reservationDto, room!);
            await SaveReservationAndUpdateRoom(reservation, room);
            ScheduleReservationExpiration(reservation.Id);

            var response = BuildReservationResponse(reservation, room);
            return ResponseDto<ReservationResponseDto>.Success(response,
                $"Reservation created successfully! Please complete the payment within {ReservationExpirationMinutes} minutes.");
        }

        private IAsyncPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<DbUpdateConcurrencyException>()
                .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt =>
                    TimeSpan.FromMilliseconds(BaseRetryDelayMilliseconds * retryAttempt) +
                    TimeSpan.FromMilliseconds(new Random().Next(0, MaxRetryJitterMilliseconds)));
        }

        private async Task<ResponseDto<ReservationResponseDto>?> ValidateRoomReservationDate(ReservationDto reservationDto)
        {
            var isAvailable = await IsRoomAvailable(
                reservationDto.RoomId,
                reservationDto.CheckInDate,
                reservationDto.CheckOutDate);

            if (!isAvailable)
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.RoomNotAvailable,
                    "The room is already booked for the selected period.");

            return null;
        }



        private async Task<Reservation> CreateReservationEntity(ReservationDto reservationDto, GetRoomResponseDto room)
        {
            var reservation = _mapper.Map<Reservation>(reservationDto);
            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(ReservationExpirationMinutes);
            reservation.ReservationStatusId = ReservationStatusCode.Pending;
            var numberOfNights = (decimal)(reservationDto.CheckOutDate - reservationDto.CheckInDate).TotalDays;
            reservation.Discount = await GetApplicableDiscount(reservationDto);
            reservation.TotalPrice = CalculateTotalPrice(room.PricePerNight, numberOfNights, reservation.Discount ?? 0);

            return reservation;
        }

        private decimal CalculateTotalPrice(decimal pricePerNight, decimal numberOfNights, decimal discountPercentage)
        {
            var subtotal = pricePerNight * numberOfNights;
            return discountPercentage > 0
                ? subtotal * (1 - discountPercentage / 100)
                : subtotal;
        }

        private async Task SaveReservationAndUpdateRoom(Reservation reservation, GetRoomResponseDto room)
        {
            // Concurrency-checked operation FIRST — if this throws
            // DbUpdateConcurrencyException, no reservation was saved yet
            await _roomServices.UpdateRoom(room.Id, new UpdateRoomRequestDto
            {
                
                RowVersion = room.RowVersion
            });

            await _reservationRepository.Add(reservation);
        }

        private void ScheduleReservationExpiration(Guid reservationId)
        {
            _backgroundJobService.ScheduleReservationCancellation(
                reservationId,
                TimeSpan.FromMinutes(ReservationExpirationMinutes));
        }

        private ReservationResponseDto BuildReservationResponse(Reservation reservation, GetRoomResponseDto room)
        {
            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.RoomNumber = room.RoomNumber;
            response.Status = reservation.ReservationStatusId.ToString();
            return response;
        }

        private async Task<decimal> GetApplicableDiscount(ReservationDto dto)
        {
            if (dto.OfferId is not { } offerId)
                return 0;

            var roomOffer = await _RoomOffersServices.GetOfferForRoomAsync(dto.RoomId, offerId);

            if (roomOffer?.Data is not { } offer)
                return 0;

            var isOfferValid = offer.IsActive &&
                               offer.StartDate <= dto.CheckInDate &&
                               offer.EndDate >= dto.CheckInDate;

            return isOfferValid ? offer.DiscountPercentage : 0;
        }

        public async Task<bool> IsRoomAvailable(Guid roomId, DateTime checkIn, DateTime checkOut)
        {
            return !await _reservationRepository
                .IsExist(r => r.RoomId == roomId &&
                              r.ReservationStatusId != ReservationStatusCode.Cancelled &&
                              r.ReservationStatusId != ReservationStatusCode.CheckedOut &&
                              checkIn < r.CheckOutDate &&
                              checkOut > r.CheckInDate);
        }



        public async Task CheckAndCancelReservation(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetbyId(reservationId).FirstOrDefaultAsync();

            if (reservation == null || reservation.ReservationStatusId != ReservationStatusCode.Pending)
                return;

            await CancelReservation(reservation);

        }

        private async Task CancelReservation(Reservation reservation)
        {
            reservation.ReservationStatusId = ReservationStatusCode.Cancelled;
            await _reservationRepository.UpdateIncludeAsync(reservation, nameof(Reservation.ReservationStatusId));
        }



        public async Task<ResponseDto<ReservationResponseDto>> GetReservationById(Guid reservationId)
        {
            var reservation = _reservationRepository.GetbyId(reservationId).FirstOrDefault();

            if (reservation == null)
            {
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.NotFound,
                    "Reservation not found.");
            }

            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.Status = reservation.ReservationStatusId.ToString();

            return ResponseDto<ReservationResponseDto>.Success(response);
        }

        public async Task<bool> IsReservationExist(Guid id) => await _reservationRepository.IsExist(x => x.Id == id);



        private async Task<(GetRoomResponseDto? Room, ResponseDto<ReservationResponseDto>? Error)> ValidateAndGetRoom(Guid roomId)
        {
            var room = (await _roomServices.GetRoomById(roomId)).Data;

            if (room == null)
                return (null, ResponseDto<ReservationResponseDto>.Fail(ErrorCode.RoomNotFound,
                    "The room is currently not available for booking."));

            if (!room.IsAvailable)
                return (null, ResponseDto<ReservationResponseDto>.Fail(ErrorCode.RoomNotAvailable,
                    "The room is currently not available for booking."));

            return (room, null);
        }
    }




}
