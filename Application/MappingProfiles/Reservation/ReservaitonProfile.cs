using Application.Dtos.Reservation;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
namespace Application.MappingProfiles.Reservation
{
    public class ReservaitonProfile : Profile
    {
        public ReservaitonProfile()
        {

            CreateMap<ReservationDto, Domain.Models.Reservation>();
            CreateMap<Domain.Models.Reservation , ReservationResponseDto>();

        }
    }
}
