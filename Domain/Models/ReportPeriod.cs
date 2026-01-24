using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ReportPeriod
    {
        [Key]
        public ReportPeriodCode Id { get; set; }
        public string Name { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;
    }
}
