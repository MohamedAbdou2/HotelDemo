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

     
        [MaxLength(200)]
        public string? TransactionId { get; set; }

    
        [Column(TypeName = "nvarchar(max)")]
        public string? GatewayResponse { get; set; }

   
        [MaxLength(500)]
        public string? PaymentUrl { get; set; }

        // ==================== Security & Tracking ====================

        
        [MaxLength(45)]  // IPv6 max length
        public string? IpAddress { get; set; }

      
        public bool WebhookVerified { get; set; } = false;

        // ==================== Timestamps ====================

        public DateTime? CompletedAt { get; set; }

       
        public DateTime? FailedAt { get; set; }

        // ==================== Failure Info ====================

        [MaxLength(500)]
        public string? FailureReason { get; set; }

   
        [MaxLength(100)]
        public string? FailureCode { get; set; }

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


      
    }
}