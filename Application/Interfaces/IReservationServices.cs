using Application.Dtos;
using Application.Dtos.Reservation;

namespace Application.Interfaces
{
    public interface IReservationServices
    {
       
        Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto);
    }
}
