using Application.Dtos;
using Application.Dtos.Reservation;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;


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

        private async Task<bool> RoomAvailableAsync(Guid roomId)
        {
            var room = await _roomRepository.GetbyId(roomId);
            return room.Any(r => r.IsAvailable == true);
        }







        /*   Background Service: تعمل كل 10 دقائق(باستخدام IHostedService أو Hangfire).

   الوظيفة: تبحث عن أي حجز حالته Pending ومر على إنشائه أكثر من 20 دقيقة، وتقوم بتغيير حالته إلى Cancelled أو Expired لتعود الغرفة متاحة للآخرين.*/
    }




}
