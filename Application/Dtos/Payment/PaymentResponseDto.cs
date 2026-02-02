using Domain.Enums;

namespace Application.Dtos.Payment
{
    /// <summary>
    /// DTO for payment response
    /// </summary>
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }

        public PaymentMethodCode PaymentMethodId { get; set; }
        public PaymentStatusCode PaymentStatusId { get; set; }

        public string? TransactionId { get; set; }
        public string? GatewayResponse { get; set; }

        public DateTime? CompletedAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public DateTime? RefundedAt { get; set; }

        public string? FailureReason { get; set; }
        public string? FailureCode { get; set; }

        public DateTime CreatedAt { get; set; }

        // Computed properties for convenience
        public bool IsSuccessful => PaymentStatusId == PaymentStatusCode.Paid;
        public bool IsPending => PaymentStatusId == PaymentStatusCode.Pending;
        public bool IsFailed => PaymentStatusId == PaymentStatusCode.Failed;
        public bool IsRefunded => PaymentStatusId == PaymentStatusCode.Refunded;
    }
}
