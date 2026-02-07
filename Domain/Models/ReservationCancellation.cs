using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ReservationCancellation
    {
        [Key]
        public Guid ReservationId { get; set; }  // PK and FK

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; }

        [Required]
        public Guid CancelledByUserId { get; set; }

        [Required]
        public DateTime CancelledAt { get; set; }

       
        public  CancellationType ? CancellationType { get; set; }  // Auto, Manual, Admin

        public bool RefundIssued { get; set; }

        public decimal? RefundAmount { get; set; }

        // Navigation
        public virtual Reservation Reservation { get; set; }
        public virtual User CancelledByUser { get; set; }
        public DateTime? RefundProcessedAt { get; set; }
    }
}