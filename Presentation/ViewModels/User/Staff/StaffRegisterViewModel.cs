namespace Presentation.ViewModels.User.Staff
{
    public class StaffRegisterViewModel
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




