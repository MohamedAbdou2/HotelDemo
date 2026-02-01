namespace Domain.Models
{
    public class UserOtp : BaseModel
    {
        public Guid UserId { get; set; }
        public string otp { get; set; }
        public DateTime ExpiresAt { get; set; }
        public User User { get; set; }
    }
}
