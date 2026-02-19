namespace Presentation.ViewModels.Feedback
{
    public class CreateFeedbackViewModel
    {
        public Guid ReservationId { get; set; }

        public float Rating { get; set; }

        public string Comments { get; set; } = null!;
    }
}
