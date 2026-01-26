using Application.Dtos;
using Application.Dtos.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReservationServices
    {
       
        Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto);
    }
}
