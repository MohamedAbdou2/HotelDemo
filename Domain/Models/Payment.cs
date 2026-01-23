using Domain.Enums;

namespace Domain.Models
{
    public class Payment : BaseModel
    {
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public PaymentMethodCode PaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = null!;

        public PaymentStatusCode PaymentStatusId { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = null!;
    }
}
