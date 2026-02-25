using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Offers
{
    public class RoomOfferResponseDto
    {
        public Guid RoomId { get; set; }
        public Guid OfferId { get; set; }
        public string Title { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
