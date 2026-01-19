using HotelDemo.Data.Enums;

namespace HotelDemo.Data.Models
{
    public class ReservationStatus
    {
        public ReservationStatusCode Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;

    }
}
