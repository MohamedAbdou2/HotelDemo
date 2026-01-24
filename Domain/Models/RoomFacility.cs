using Domain.Enums;

namespace Domain.Models
{
    public class RoomFacility
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public RoomFacilityCode FacilityId { get; set; }
        public Facility Facility { get; set; } = null!;
    }
}
