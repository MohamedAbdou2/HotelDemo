using Domain.Enums;

namespace Presentation.ViewModels.Room
{
    public class RoomFilterRequestViewModel
    {
        public RoomTypeCode? RoomTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

    }
}
