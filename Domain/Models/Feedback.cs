
namespace Domain.Models
{
    public class Feedback : BaseModel
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid ReservationId { get; set; }

        public Reservation Reservation { get; set; } = null!;

        public float Rating { get; set; }

        public string Comments { get; set; } = null!;





    }
}
