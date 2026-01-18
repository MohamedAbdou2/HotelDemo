namespace HotelDemo.Data.Models
{
    public class Feedback : BaseModel
    {
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid RoomId { get; set; }

        public Room Room { get; set; } = null!;

        public int Rating { get; set; }

        public string Comments { get; set; } = null!;

        public FeedbackResponse? FeedbackResponse { get; set; }




    }
}
