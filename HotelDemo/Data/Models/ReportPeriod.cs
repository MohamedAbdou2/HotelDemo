using HotelDemo.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelDemo.Data.Models
{
    public class ReportPeriod
    {
        [Key]
        public ReportPeriodCode Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
