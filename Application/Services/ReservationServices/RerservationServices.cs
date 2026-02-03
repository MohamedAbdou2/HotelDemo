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
    public class RerservationServices : IReservationServices
    {
        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ReservationDto> _reservationValidator;
        private readonly IBackgroundJobService _backgroundJobService;   

        public RerservationServices(IGenericRepository<Room> roomRepository,
            IGenericRepository<Reservation> reservationRepository,
            IGenericRepository<Payment> paymentRepository,
            IMapper mapper,
            IValidator<ReservationDto> reservationValidator,
            IBackgroundJobService backgroundJobService)
        {

            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _reservationValidator = reservationValidator;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto)
        {
            var validationResult = _reservationValidator.Validate(reservationDto);
            if (!validationResult.IsValid)
            {
                return ResponseDto<ReservationResponseDto>.ValidaitonFail(validationResult);
            }

            var isAvailable = await IsRoomAvailable(reservationDto.RoomId, reservationDto.CheckInDate, reservationDto.CheckOutDate);
            if (!isAvailable)
            {
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.RoomNotAvailable,
                    "The room is already booked for the selected period.");
            }

            var roomQuery = await _roomRepository.GetbyId(reservationDto.RoomId);
            var room = roomQuery.FirstOrDefault();
            if (room == null || !room.IsAvailable)
            {
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.RoomNotFound,
                    "The room is currently not available for booking.");
            }

            var reservation = _mapper.Map<Reservation>(reservationDto);
            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
            reservation.ReservationStatusId = ReservationStatusCode.Pending;

            await _reservationRepository.Add(reservation);

            room.IsAvailable = false;
            await _roomRepository.UpdateIncludeAsync(room , x=>x.IsAvailable);

            _backgroundJobService.ScheduleReservationCancellation(reservation.Id, TimeSpan.FromMinutes(10));

            var response = _mapper.Map<ReservationResponseDto>(reservation);
            response.RoomNumber = room.RoomNumber;
            response.Status = reservation.ReservationStatusId.ToString();

            return ResponseDto<ReservationResponseDto>.Success(response,
                "Reservation created successfully! Please complete the payment within 10 minutes.");
        }
        public async Task<ResponseDto<bool>> BookRoomAsync(ReservationDto dto)
        {
            
            var retryPolicy = Policy
                .Handle<DbUpdateConcurrencyException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromMilliseconds(200 * retryAttempt) + TimeSpan.FromMilliseconds(new Random().Next(0, 50)));

            return await retryPolicy.ExecuteAsync(async () =>
            {
                
                 
                    var isAvailable = await IsRoomAvailable(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
                    if (!isAvailable)
                        return ResponseDto<bool>.Fail(ErrorCode.RoomNotAvailable, "The room is already booked for the selected period.");

                
                    var roomQuery = await _roomRepository.GetbyId(dto.RoomId);
                    var room = roomQuery.FirstOrDefault();
                     if (room == null || ! room.IsAvailable) 
                        return ResponseDto<bool>.Fail(ErrorCode.RoomNotFound, "The room is currently not available for booking.");

              
                    var reservation = _mapper.Map<Reservation>(dto);
                    await _reservationRepository.Add(reservation);

                room.IsAvailable = false;
                await _roomRepository.Update(room);
                _backgroundJobService.ScheduleReservationCancellation(reservation.Id, TimeSpan.FromMinutes(10));
                return ResponseDto<bool>.Success(true, "Reservation created successfully! Please complete the payment within 10 minutes.");
                
            });
        }
        private async Task<bool> RoomAvailableAsync(Guid roomId)
        {
            var room = await _roomRepository.GetbyId(roomId);
            return room.Any(r => r.IsAvailable == true);
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

            if (reservation != null && reservation.ReservationStatusId == ReservationStatusCode.Pending)
            {
                reservation.ReservationStatusId = ReservationStatusCode.Cancelled;
                await _reservationRepository.UpdateIncludeAsync(reservation, nameof(Reservation.ReservationStatusId));

                var roomQuery = await _roomRepository.GetbyId(reservation.RoomId);
                var room = roomQuery.FirstOrDefault();
                if (room != null)
                {
                    room.IsAvailable = true;
                    await _roomRepository.Update(room);
                }
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
