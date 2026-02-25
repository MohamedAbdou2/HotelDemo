using Domain.Enums;
using Domain.Models;

namespace Application.Dtos.Reservation
{
    public class ReservationDto
    {
        public Guid RoomId { get; set; }
        public Guid? OfferId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationStatusCode? ReservationStatusId { get; set; } = ReservationStatusCode.Pending;
        public Guid? PaymentId { get; set; }
    }
}