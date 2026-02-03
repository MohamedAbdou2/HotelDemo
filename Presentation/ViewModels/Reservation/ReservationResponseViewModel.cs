namespace Presentation.ViewModels.Reservation
{
    /// <summary>
    /// View model for reservation response (output to client)
    /// </summary>
    public class ReservationResponseViewModel
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalPrice { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public string? ExpiresIn { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string GetDetailsUrl { get; set; } = string.Empty;
    }
}
