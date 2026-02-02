using Domain.Enums;

namespace Application.Dtos.Payment
{
    /// <summary>
    /// DTO for initiating a payment request
    /// </summary>
    public class InitiatePaymentRequestDto
    {
        public Guid ReservationId { get; set; }
        public Guid CustomerId { get; set; }
        public PaymentMethodCode PaymentMethodId { get; set; } = PaymentMethodCode.Stripe;
        public string? IpAddress { get; set; }
    }
}
