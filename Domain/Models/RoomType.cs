using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class RoomType
    {
        [Key]
        public RoomTypeCode Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal PriceMultiplier { get; set; }
        public bool IsAvailable { get; set; } = true;
        public ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
    }
}
