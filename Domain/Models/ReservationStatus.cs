using Domain.Enums;

namespace Domain.Models
{
    public class ReservationStatus
    {
        public ReservationStatusCode Id { get; set; }

        public string Name { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;

    }
}
