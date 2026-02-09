using Application.Dtos.Room;

namespace Presentation.ViewModels.Offer;

public class OfferResponseViewModel
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
