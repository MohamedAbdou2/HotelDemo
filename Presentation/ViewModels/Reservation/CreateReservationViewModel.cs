namespace Presentation.ViewModels.Reservation
{
    /// <summary>
    /// View model for creating a reservation (input from client)
    /// </summary>
    public class CreateReservationViewModel
    {
        public Guid RoomId { get; set; }
        public Guid ? CustomerId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
