namespace HotelDemo.Data.Models
{
    public class User : BaseModel
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        
        public ICollection <UserRole> UserRoles { get; set; } = new HashSet<UserRole>();



    }
}
