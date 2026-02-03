using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models
{
    public class Refund : BaseModel
    {
        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public RefundStatusCode RefundStatusId { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; }

        [MaxLength(200)]
        public string? GatewayRefundId { get; set; }

        [MaxLength(500)]
        public string? FailureReason { get; set; }

        public DateTime? ProcessedAt { get; set; }
        public DateTime? FailedAt { get; set; }

        public Guid? ProcessedByUserId { get; set; }

        // Navigation Properties
        public Payment Payment { get; set; }
        public User ProcessedByUser { get; set; }
    }
}
