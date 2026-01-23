using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class PaymentMethod
    {
        [Key]
        public PaymentMethodCode Id { get; set; }
        public string Name { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;

    }
}
