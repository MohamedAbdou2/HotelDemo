using Domain.Enums;

namespace Application.Dtos.Reservation
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public ReservationStatusCode ReservationStatusId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}