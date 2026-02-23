
namespace Domain.Models
{
    public abstract class BaseModel
    {
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

        public User CreatedBy { get; set; } = null!;

        public User? UpdatedBy { get; set; }


    }
}
