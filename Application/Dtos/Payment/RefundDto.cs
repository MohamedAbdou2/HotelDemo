using Domain.Enums;

namespace Application.Dtos.Payment
{
    public class RefundRequestDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }

    public class RefundResponseDto
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public RefundStatusCode RefundStatusId { get; set; }
        public string Reason { get; set; }
        public string? GatewayRefundId { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public Guid? ProcessedByUserId { get; set; }
    }
}
