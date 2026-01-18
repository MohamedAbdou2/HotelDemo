namespace HotelDemo.Data.Models
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

        // TODo: reservation status enum 
        // public ReservationStatus Status { get; set; }
        //ToDo : one to one 
        //public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();

    }
}
