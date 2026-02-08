using Application.Dtos.Reservation;

namespace Application.Interfaces
{
    public interface IReservationServices
    {
        Task<ReservationResponseDto> CreateReservation(ReservationDto reservationDto);
    }
}
