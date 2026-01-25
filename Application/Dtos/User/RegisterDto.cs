using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.User
{
    public class RegisterDto
    {
        public string userName { get; set; }

        public int phoneNumber { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string confirmPassword { get; set; }
    }
}
