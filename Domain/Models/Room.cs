using Domain.Enums;

namespace Domain.Models
{
    public class Room : BaseModel
    {
        public string RoomNumber { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;

        public ICollection<RoomPicture> RoomPictures { get; set; } = new HashSet<RoomPicture>();

        public ICollection<RoomFacility> RoomFacilities { get; set; } = new HashSet<RoomFacility>();

        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>();
        public ICollection<RoomOffer> RoomOffers { get; set; } = new HashSet<RoomOffer>();

        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public RoomTypeCode RoomTypeId { get; set; }
        public RoomType Type { get; set; } = null!;


    }
}
