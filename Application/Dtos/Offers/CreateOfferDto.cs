using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Offers
{
    public class CreateOfferDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        public bool? IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Guid> RoomIds { get; set; } = new List<Guid>();

    }
}
