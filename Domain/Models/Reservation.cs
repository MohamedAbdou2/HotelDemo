using Domain.Enums;

namespace Domain.Models
{
    public class Reservation : BaseModel
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }

        public ReservationStatusCode ReservationStatusId { get; set; }
        public ReservationStatus ReservationStatus { get; set; } = null!;

        public Guid? PaymentId { get; set; }
        public Payment? Payment { get; set; }

        ICollection<Feedback> Feedbacks { get; set; }

    }
}
