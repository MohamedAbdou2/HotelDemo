
namespace Domain.Models
{
    public class RoomOffer : BaseModel
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public Guid OfferId { get; set; }
        public Offer Offer { get; set; } = null!;
    }
}
