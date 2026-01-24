using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PaymentStatus
    {
        [Key]
        public PaymentStatusCode Id { get; set; }
        public string Name { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;

    }
}
