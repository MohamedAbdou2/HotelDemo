namespace HotelDemo.Data.Models
{
    public class Staff : BaseModel
    {

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Position { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

    }
}
