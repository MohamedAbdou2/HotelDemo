using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.User
{
    public class ResetPasswordDto
    {
        public string otp { get; set; }

        public string password { get; set; }

        public string confirmPassword { get; set; }
    }
}
