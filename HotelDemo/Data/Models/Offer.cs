namespace HotelDemo.Data.Models
{
    public class Offer : BaseModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal DiscountPercentage { get; set; }

         public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<RoomOffer> RoomOffers { get; set; } = new HashSet<RoomOffer>();

    }
}
