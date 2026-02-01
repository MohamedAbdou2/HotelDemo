using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Dtos.User.Staff
{
    public class StaffRegisterDto : RegisterDto
    {
      
        public string Position { get; set; } = null!;
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public DateTime? TerminationDate { get; set; }

    }
}
