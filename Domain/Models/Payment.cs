using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Payment : BaseModel
    {
        // ==================== Basic Info ====================

        [Required]
        public Guid ReservationId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }  

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // ==================== Status & Method ====================

        [Required]
        public PaymentMethodCode PaymentMethodId { get; set; }

        [Required]
        public PaymentStatusCode PaymentStatusId { get; set; }

        // ==================== Gateway Info ====================

        /// <summary>
        /// Unique transaction ID from payment gateway (e.g., Paymob)
        /// Used for idempotency and refunds
        /// </summary>
        [MaxLength(200)]
        public string? TransactionId { get; set; }

        /// <summary>
        /// Complete gateway response in JSON format
        /// Useful for debugging and customer support
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? GatewayResponse { get; set; }

        /// <summary>
        /// Payment URL from gateway (e.g., Stripe Checkout URL)
        /// Customer uses this URL to complete payment
        /// </summary>
        [MaxLength(500)]
        public string? PaymentUrl { get; set; }

        // ==================== Security & Tracking ====================

        /// <summary>
        /// Customer IP address for fraud detection
        /// </summary>
        [MaxLength(45)]  // IPv6 max length
        public string? IpAddress { get; set; }

        /// <summary>
        /// Whether webhook signature was verified
        /// </summary>
        public bool WebhookVerified { get; set; } = false;

        // ==================== Timestamps ====================

        // ✅ CreatedAt موجود في BaseModel

        /// <summary>
        /// When payment was successfully completed
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// When payment failed
        /// </summary>
        public DateTime? FailedAt { get; set; }

        /// <summary>
        /// When payment was refunded
        /// </summary>
        public DateTime? RefundedAt { get; set; }

        // ==================== Failure Info ====================

        /// <summary>
        /// Human-readable failure reason
        /// </summary>
        [MaxLength(500)]
        public string? FailureReason { get; set; }

        /// <summary>
        /// Gateway-specific error code
        /// </summary>
        [MaxLength(100)]
        public string? FailureCode { get; set; }

        // ==================== Refund Info (Legacy) ====================
        // ⚠️ Kept for backward compatibility - Use Refund table instead

        [MaxLength(500)]
        [Obsolete("Use Refund table instead")]
        public string? RefundReason { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Obsolete("Use Refunds collection instead")]
        public decimal? RefundAmount { get; set; }

        [MaxLength(200)]
        [Obsolete("Use Refund.GatewayRefundId instead")]
        public string? RefundTransactionId { get; set; }

        // ==================== Computed Properties ====================

        /// <summary>
        /// Total amount refunded from all refunds
        /// </summary>
        [NotMapped]
        public decimal TotalRefunded => Refunds?.Where(r => r.RefundStatusId == RefundStatusCode.Completed).Sum(r => r.Amount) ?? 0;

        /// <summary>
        /// Remaining amount that can be refunded
        /// </summary>
        [NotMapped]
        public decimal RefundableAmount => Amount - TotalRefunded;

        // ==================== Concurrency Control ====================

        /// <summary>
        /// For optimistic concurrency control
        /// Prevents race conditions in payment updates
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

        // ==================== Navigation Properties ====================

        public virtual Reservation Reservation { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
        public virtual PaymentStatus PaymentStatus { get; set; } = null!;

        // ==================== Computed Properties ====================

        /// <summary>
        /// Quick check if payment succeeded
        /// </summary>
        [NotMapped]
        public bool IsSuccessful => PaymentStatusId == PaymentStatusCode.Paid;

        /// <summary>
        /// Quick check if payment is still pending
        /// </summary>
        [NotMapped]
        public bool IsPending => PaymentStatusId == PaymentStatusCode.Pending;

        /// <summary>
        /// Quick check if payment was refunded
        /// </summary>
        [NotMapped]
        public bool IsRefunded => RefundedAt.HasValue;

        /// <summary>
        /// Quick check if payment failed
        /// </summary>
        [NotMapped]
        public bool IsFailed => PaymentStatusId == PaymentStatusCode.Failed;

        /// <summary>
        /// How long it took to process the payment
        /// </summary>
        [NotMapped]
        public TimeSpan? ProcessingTime => CompletedAt.HasValue
            ? CompletedAt.Value - CreatedAt
            : null;

        /// <summary>
        /// Percentage of amount that was refunded
        /// </summary>
        [NotMapped]
        public decimal? RefundPercentage => RefundAmount.HasValue && Amount > 0
            ? (RefundAmount.Value / Amount) * 100
            : null;
    }
}