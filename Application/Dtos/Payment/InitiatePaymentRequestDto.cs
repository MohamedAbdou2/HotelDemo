using Domain.Enums;

namespace Application.Dtos.Payment
{
    public class InitiatePaymentRequestDto
    {
        public Guid ReservationId { get; set; }
        public PaymentMethodCode PaymentMethodId { get; set; } = PaymentMethodCode.Stripe;
    }
}
