using Application.Dtos;
using Application.Dtos.Payment;

namespace Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ResponseDto<PaymentResponseDto>> InitiatePaymentAsync(Guid reservationId, string? ipAddress = null);
        Task<ResponseDto<PaymentResponseDto>> VerifyPaymentAsync(Guid paymentId, string transactionId);
        Task<ResponseDto<string>> HandleWebhookAsync(string webhookData);
        Task<ResponseDto<List<PaymentResponseDto>>> GetPaymentHistoryAsync(Guid reservationId);
        Task<ResponseDto<PaymentResponseDto>> RefundPaymentAsync(Guid paymentId, string refundReason);
    }
}
