
namespace Domain.Models
{
    public class FeedbackResponse : BaseModel
    {
        public Guid FeedbackId { get; set; }
        public Feedback Feedback { get; set; } = null!;
        public string ResponseText { get; set; } = null!;
        public DateTime ResponseDate { get; set; }
    }
}
