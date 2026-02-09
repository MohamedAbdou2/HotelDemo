namespace Application.Dtos.Offers
{
    public class CreateOfferDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Guid> RoomIds { get; set; } = new List<Guid>();

    }
}
