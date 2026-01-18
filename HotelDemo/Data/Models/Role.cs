using HotelDemo.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelDemo.Data.Models
{
    public class Role
    {
        [Key]
        public UserRoleCode Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    }
}
