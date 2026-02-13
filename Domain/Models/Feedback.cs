
namespace Domain.Models
{
    public class Feedback : BaseModel
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid ReservationId { get; set; }

        public Room Reservation { get; set; } = null!;

        public int Rating { get; set; }

        public string Comments { get; set; } = null!;

        //public FeedbackResponse? FeedbackResponse { get; set; }




    }
}
