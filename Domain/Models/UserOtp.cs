using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserOtp:BaseModel
    {
        public Guid UserId { get; set; }
        public string otp { get; set; }
        public DateTime ExpiresAt { get; set; }
        public User User { get; set; }
    }
}
