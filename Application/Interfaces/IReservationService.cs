using Application.Dtos;
using Application.Dtos.Reservation;

namespace Application.Interfaces
{
    public interface IReservationService
    {
        Task<ResponseDto<ReservationResponseDto>> CreateReservation(ReservationDto reservationDto);
        Task<ResponseDto<ReservationResponseDto>> GetReservationById(Guid reservationId);
        Task CheckAndCancelReservation(Guid reservationId);

        Task<bool> IsReservationExist(Guid id);
    }
}
