using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Dtos.User.Staff
{
    public class StaffRegisterDto
    {
        public string userName { get; set; }

        public int phoneNumber { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string confirmPassword { get; set; }
        public string Position { get; set; } = null!;
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public DateTime? TerminationDate { get; set; }

    }
}
