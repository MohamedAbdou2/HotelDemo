using Domain.Enums;

namespace Application.Dtos.Room
{
    public class RoomFilterRequestDto
    {
        public RoomTypeCode? RoomTypeId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;


    }
}
