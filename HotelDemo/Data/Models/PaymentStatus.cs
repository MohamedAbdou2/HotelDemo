using HotelDemo.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelDemo.Data.Models
{
    public class PaymentStatus
    {
        [Key]
        public PaymentStatusCode Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
