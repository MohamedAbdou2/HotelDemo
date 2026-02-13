using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Reservation : BaseModel
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? Discount { get; set; }   
        public ReservationStatusCode ReservationStatusId { get; set; }
        public ReservationStatus ReservationStatus { get; set; } = null!;

        /// <summary>
        /// Payments related to this reservation
        /// A reservation can have multiple payment attempts
        /// </summary>
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();


        // ==================== Status & Timestamps ====================


        /// <summary>
        /// Reservation expiration time (10 minutes from creation)
        /// Used by Hangfire to auto-cancel unpaid reservations
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public DateTime? CheckedInAt { get; set; }

        public DateTime? CheckedOutAt { get; set; }

        // ==================== Cancellation Info ====================

        
        public virtual ReservationCancellation? Cancellation { get; set; }

        // ==================== Concurrency Control ====================

        /// <summary>
        /// RowVersion for optimistic concurrency control
        /// Prevents double booking and race conditions
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }


        ICollection<Feedback> Feedbacks { get; set; }

    }



}
