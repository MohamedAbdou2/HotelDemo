using Application.Dtos;
using Application.Dtos.Reservation;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto)
        {
            var validationResult = _reservationValidator.Validate(reservationDto);
            /*if (!validationResult.IsValid)
            {
                return;
            }*/
            throw new NotImplementedException();
        }
     /*   Background Service: تعمل كل 10 دقائق(باستخدام IHostedService أو Hangfire).

الوظيفة: تبحث عن أي حجز حالته Pending ومر على إنشائه أكثر من 20 دقيقة، وتقوم بتغيير حالته إلى Cancelled أو Expired لتعود الغرفة متاحة للآخرين.*/
    }
}
