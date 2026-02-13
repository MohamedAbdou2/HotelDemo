using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Feedback
{
    public class FeedbackDto
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid ReservationId { get; set; }

        public int Rating { get; set; }

        public string Comments { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string? CustomerName { get; set; }
    }
}
