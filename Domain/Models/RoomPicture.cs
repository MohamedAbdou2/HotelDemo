
namespace Domain.Models
{
    public class RoomPicture : BaseModel
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
    }
}
