namespace Application.Dtos.User
{
    public class RegisterDto
    {
        public string userName { get; set; }

        public string phoneNumber { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string confirmPassword { get; set; }
    }
}
