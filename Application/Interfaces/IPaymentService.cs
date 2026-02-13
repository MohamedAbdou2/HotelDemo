using Application.Dtos;
using Application.Dtos.Payment;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ResponseDto<PaymentResponseDto>> InitiatePaymentAsync(Guid reservationId, string? ipAddress = null);
        Task<ResponseDto<PaymentResponseDto>> HandleStripeSuccessAsync(Guid paymentId);
        Task<ResponseDto<string>> HandleStripeCancelAsync(Guid paymentId);
        Task<ResponseDto<string>> HandleStripeWebhookAsync(string json, string stripeSignature);
        Task<ResponseDto<List<PaymentResponseDto>>> GetPaymentHistoryAsync(Guid reservationId);
    }
}
