using HotelDemo.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelDemo.Data.Models
{
    public class PaymentMethod
    {
        [Key]
        public PaymentMethodCode Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
