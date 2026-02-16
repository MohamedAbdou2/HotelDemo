namespace Presentation.ViewModels.Feedback
{
    public class FeedbackViewModel
    {
        public Guid Id { get; set; }

        public Guid ReservationId { get; set; }

        public int Rating { get; set; }

        public string Comments { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string? CustomerName { get; set; }
    }
}
