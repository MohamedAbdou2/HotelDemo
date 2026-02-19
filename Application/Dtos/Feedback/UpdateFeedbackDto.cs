using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Feedback
{
    public class UpdateFeedbackDto
    {
        public float Rating { get; set; }

        public string Comments { get; set; } = null!;
    }
}
