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

        // ==================== Refund Info ====================

        /// <summary>
        /// Reason for refund
        /// </summary>
        [MaxLength(500)]
        public string? RefundReason { get; set; }

        /// <summary>
        /// Amount refunded (can be partial)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }

        /// <summary>
        /// Transaction ID for the refund operation
        /// </summary>
        [MaxLength(200)]
        public string? RefundTransactionId { get; set; }

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