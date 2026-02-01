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

        public RerservationServices(IGenericRepository<Room> roomRepository,
            IGenericRepository<Reservation> reservationRepository,
            IGenericRepository<Payment> paymentRepository,
            IMapper mapper,
            IValidator<ReservationDto> reservationValidator)
        {

            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _reservationValidator = reservationValidator;
        }

        public async Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto)
        {
            var validationResult = _reservationValidator.Validate(reservationDto);
            if (!validationResult.IsValid)
            {
                return ResponseDto<ReservationResponseDto>.ValidaitonFail(validationResult);
            }
            if (!await RoomAvailableAsync(reservationDto.RoomId))
            {
                return ResponseDto<ReservationResponseDto>.Fail(ErrorCode.NotAvailableRoom,
                    "this Room Is not availabe for reservation Now");
            }


            throw new NotImplementedException();
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
                        return ResponseDto<bool>.Fail(ErrorCode.RoomNotAvailable, "الغرفة محجوزة في هذه الفترة.");

                
                    var roomQuery = await _roomRepository.GetbyId(dto.RoomId);
                    var room = roomQuery.FirstOrDefault();
                     if (room == null || room.IsAvailable) 
                        return ResponseDto<bool>.Fail(ErrorCode.RoomNotFound, "الغرفة غير متاحة حالياً.");

              
                    var reservation = _mapper.Map<Reservation>(dto);
                    await _reservationRepository.Add(reservation);


                    await _roomRepository.Update(room);

               

                    return ResponseDto<bool>.Success(true, "تم الحجز بنجاح!");
               
               
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
                               checkIn < r.CheckOutDate && 
                               checkOut > r.CheckInDate);  
        }




        /*   Background Service: تعمل كل 10 دقائق(باستخدام IHostedService أو Hangfire).

   الوظيفة: تبحث عن أي حجز حالته Pending ومر على إنشائه أكثر من 20 دقيقة، وتقوم بتغيير حالته إلى Cancelled أو Expired لتعود الغرفة متاحة للآخرين.*/
    }




}
