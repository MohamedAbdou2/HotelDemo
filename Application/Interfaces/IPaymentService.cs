using Application.Dtos;
using Application.Dtos.Payment;

namespace Application.Interfaces
{
    /// <summary>
    /// Payment service interface for handling payment operations
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Initiate payment for a reservation
        /// </summary>
        Task<ResponseDto<PaymentResponseDto>> InitiatePaymentAsync(
            Guid reservationId, 
            Guid customerId, 
            string? ipAddress = null);

        /// <summary>
        /// Verify payment status from payment gateway
        /// </summary>
        Task<ResponseDto<PaymentResponseDto>> VerifyPaymentAsync(Guid paymentId, string transactionId);

        /// <summary>
        /// Handle webhook callback from payment gateway
        /// </summary>
        Task<ResponseDto<string>> HandleWebhookAsync(string webhookData);

        /// <summary>
        /// Get payment history for a reservation
        /// </summary>
        Task<ResponseDto<List<PaymentResponseDto>>> GetPaymentHistoryAsync(Guid reservationId);

        /// <summary>
        /// Refund a successful payment
        /// </summary>
        Task<ResponseDto<PaymentResponseDto>> RefundPaymentAsync(
            Guid paymentId, 
            string refundReason);
    }
}
