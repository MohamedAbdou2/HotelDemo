
namespace Domain.Models
{
    public class Customer : BaseModel
    {

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
    }
}
