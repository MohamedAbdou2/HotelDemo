using HotelDemo.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelDemo.Data.Models
{
    public class Facility
    {
        [Key]
        public RoomFacilityCode Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsAvailable { get; set; } = true;

        public ICollection<RoomFacility> RoomFacilities { get; set; } = new HashSet<RoomFacility>();

    }
}
