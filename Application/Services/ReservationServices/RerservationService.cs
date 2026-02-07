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
    public class RerservationService : IReservationService
    {
        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ReservationDto> _reservationValidator;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IReadOnlyRepository<RoomOffer> _roomOffersRepository;

        private const int ReservationExpirationMinutes = 10;
        private const int MaxRetryAttempts = 3;
        private const int BaseRetryDelayMilliseconds = 200;
        private const int MaxRetryJitterMilliseconds = 50;

        public RerservationService(
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
            return await retryPolicy.ExecuteAsync(async () => await CreateReservationWithRetry(reservationDto));
        }

        private async Task<ResponseDto<ReservationResponseDto>> CreateReservationWithRetry(ReservationDto reservationDto)
        {
            var validationResult = _reservationValidator.Validate(reservationDto);
            if (!validationResult.IsValid)
                return ResponseDto<ReservationResponseDto>.ValidaitonFail(validationResult);

            var availabilityCheck = await ValidateRoomAvailability(reservationDto);
            if (availabilityCheck != null)
                return availabilityCheck;

            var room = await GetRoomById(reservationDto.RoomId);
            if (room == null)
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.RoomNotFound,
                    "The room is currently not available for booking.");

            var reservation = await CreateReservationEntity(reservationDto, room);
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

        private async Task<ResponseDto<ReservationResponseDto>> ValidateRoomAvailability(ReservationDto reservationDto)
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

        private async Task<Room> GetRoomById(Guid roomId)
        {
            var roomQuery = await _roomRepository.GetbyId(roomId);
            var room = roomQuery.FirstOrDefault();
            return room?.IsAvailable == true ? room : null;
        }

        private async Task<Reservation> CreateReservationEntity(ReservationDto reservationDto, Room room)
        {
            var reservation = _mapper.Map<Reservation>(reservationDto);
            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(ReservationExpirationMinutes);
            reservation.ReservationStatusId = ReservationStatusCode.Pending;

            var discount = await GetApplicableDiscount(reservationDto.RoomId, reservationDto.CheckInDate);
            var numberOfNights = (decimal)(reservationDto.CheckOutDate - reservationDto.CheckInDate).TotalDays;

            reservation.Discount = discount;
            reservation.TotalPrice = CalculateTotalPrice(room.PricePerNight, numberOfNights, discount);

            return reservation;
        }

        private decimal CalculateTotalPrice(decimal pricePerNight, decimal numberOfNights, decimal discountPercentage)
        {
            var subtotal = pricePerNight * numberOfNights;
            return discountPercentage > 0
                ? subtotal * (1 - discountPercentage / 100)
                : subtotal;
        }

        private async Task SaveReservationAndUpdateRoom(Reservation reservation, Room room)
        {
            await _reservationRepository.Add(reservation);
           /* room.IsAvailable = false;
            await _roomRepository.UpdateIncludeAsync(room, x => x.IsAvailable);*/
        }

        private void ScheduleReservationExpiration(Guid reservationId)
        {
            _backgroundJobService.ScheduleReservationCancellation(
                reservationId,
                TimeSpan.FromMinutes(ReservationExpirationMinutes));
        }

        private ReservationResponseDto BuildReservationResponse(Reservation reservation, Room room)
        {
            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.RoomNumber = room.RoomNumber;
            response.Status = reservation.ReservationStatusId.ToString();
            return response;
        }

        private async Task<decimal> GetApplicableDiscount(Guid roomId, DateTime checkInDate)
        {
            var roomOfferQuery = await _roomOffersRepository.GetAll(ro => ro.RoomId == roomId);
            var roomOffer = await roomOfferQuery.Include(x => x.Offer).FirstOrDefaultAsync();

            if (roomOffer == null)
                return 0;

            var offer = roomOffer.Offer;
            var isOfferValid = offer.IsActive &&
                               offer.StartDate <= checkInDate &&
                               offer.EndDate >= checkInDate;

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
            var reservationQuery = await _reservationRepository.GetbyId(reservationId);
            var reservation = reservationQuery.FirstOrDefault();

            if (reservation == null || reservation.ReservationStatusId != ReservationStatusCode.Pending)
                return;

            await CancelReservation(reservation);
            await ReleaseRoom(reservation.RoomId);
        }

        private async Task CancelReservation(Reservation reservation)
        {
            reservation.ReservationStatusId = ReservationStatusCode.Cancelled;
            await _reservationRepository.UpdateIncludeAsync(reservation, nameof(Reservation.ReservationStatusId));
        }

        private async Task ReleaseRoom(Guid roomId)
        {
            var roomQuery = await _roomRepository.GetbyId(roomId);
            var room = roomQuery.FirstOrDefault();

            if (room != null)
            {
                room.IsAvailable = true;
                await _roomRepository.Update(room);
            }
        }

        public async Task<ResponseDto<ReservationResponseDto>> GetReservationById(Guid reservationId)
        {
            var reservationQuery = await _reservationRepository.GetbyId(reservationId);
            var reservation = reservationQuery.FirstOrDefault();

            if (reservation == null)
            {
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.NotFound,
                    "Reservation not found.");
            }

            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.Status = reservation.ReservationStatusId.ToString();

            return ResponseDto<ReservationResponseDto>.Success(response);
        }


    }




}
