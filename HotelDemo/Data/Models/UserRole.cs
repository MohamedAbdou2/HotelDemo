using HotelDemo.Data.Enums;

namespace HotelDemo.Data.Models
{
    public class UserRole : BaseModel
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public UserRoleCode RoleId { get; set; }
        public Role Role { get; set; } = null!;

    }
}
