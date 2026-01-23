using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Role
    {
        [Key]
        public UserRoleCode Id { get; set; }
        public string Name { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    }
}
