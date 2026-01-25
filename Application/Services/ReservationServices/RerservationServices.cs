using Application.Dtos.Reservation;
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
        private readonly IMapper mapper;
        private readonly IValidator<ReservationDto> _reservationValidator

        public Task<ReservationResponseDto> CreateReservation(ReservationDto reservationDto)
        {
            throw new NotImplementedException();
        }
    }
}
