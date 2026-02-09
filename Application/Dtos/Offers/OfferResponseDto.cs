using Application.Dtos.Room;

namespace Application.Dtos.Offers
{
    public class OfferResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<RoomDetailsDto> RoomDetails { get; set; } = new List<RoomDetailsDto>();
    }
}
