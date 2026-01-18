using HotelDemo.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace HotelDemo.Data.Models
{
    public class ReportType
    {
        [Key]
        public ReportTypeCode Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
