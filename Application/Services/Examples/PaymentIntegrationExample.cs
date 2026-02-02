using Application.Dtos;
using Application.Dtos.Payment;
using Application.Dtos.Reservation;
using Application.Interfaces;
using Domain.Enums;

namespace Application.Services.Examples
{
    /// <summary>
    /// Example of how to integrate payment into reservation flow
    /// This is educational - shows the complete flow
    /// </summary>
    public class PaymentIntegrationExample
    {
        private readonly IReservationServices _reservationService;
        private readonly IPaymentService _paymentService;

        public PaymentIntegrationExample(
            IReservationServices reservationService,
            IPaymentService paymentService)
        {
            _reservationService = reservationService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Complete workflow: Create Reservation → Initiate Payment
        /// </summary>
        public async Task<ResponseDto<string>> CompleteReservationWithPaymentAsync(
            ReservationDto reservationDto,
            Guid customerId,
            Guid reservationId,
            string? userIpAddress = null)
        {
            try
            {
                // Step 1: Create reservation
                var reservationResult = await _reservationService.CreateReservation(reservationDto);

                if (!reservationResult.IsSuccess)
                {
                    return ResponseDto<string>.Fail(
                        reservationResult.ErrorCode,
                        "Failed to create reservation");
                }

                // Step 2: Initiate payment
                var paymentResult = await _paymentService.InitiatePaymentAsync(
                    reservationId,
                    customerId,
                    userIpAddress);

                if (!paymentResult.IsSuccess)
                {
                    return ResponseDto<string>.Fail(
                        paymentResult.ErrorCode,
                        "Failed to initiate payment");
                }

                // Step 3: Return payment details to client
                // Client will use this to redirect to payment gateway
                return ResponseDto<string>.Success(
                    paymentResult.Data.Id.ToString(),
                    "Reservation created. Please proceed to payment.");
            }
            catch (Exception ex)
            {
                return ResponseDto<string>.Fail(
                    ErrorCode.ServerError,
                    $"Unexpected error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle payment success callback
        /// Called after user completes payment on gateway
        /// </summary>
        public async Task<ResponseDto<string>> HandlePaymentSuccessAsync(
            Guid paymentId,
            string transactionId)
        {
            try
            {
                var verifyResult = await _paymentService.VerifyPaymentAsync(
                    paymentId,
                    transactionId);

                if (!verifyResult.IsSuccess)
                {
                    return ResponseDto<string>.Fail(
                        verifyResult.ErrorCode,
                        "Payment verification failed");
                }

                return ResponseDto<string>.Success(
                    "Payment confirmed successfully. Your reservation is confirmed!");
            }
            catch (Exception ex)
            {
                return ResponseDto<string>.Fail(
                    ErrorCode.ServerError,
                    $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get payment history for a reservation
        /// </summary>
        public async Task<ResponseDto<List<PaymentResponseDto>>> GetReservationPaymentHistoryAsync(
            Guid reservationId)
        {
            return await _paymentService.GetPaymentHistoryAsync(reservationId);
        }

        /// <summary>
        /// Cancel reservation and refund payment
        /// </summary>
        public async Task<ResponseDto<string>> CancelReservationWithRefundAsync(
            Guid reservationId,
            Guid paymentId,
            string cancellationReason)
        {
            try
            {
                // Get payment history
                var paymentsResult = await _paymentService.GetPaymentHistoryAsync(reservationId);
                
                if (!paymentsResult.IsSuccess || !paymentsResult.Data.Any())
                {
                    return ResponseDto<string>.Fail(
                        ErrorCode.NotFound,
                        "No payment found");
                }

                var paidPayment = paymentsResult.Data.FirstOrDefault(p => p.IsSuccessful);
                
                if (paidPayment == null)
                {
                    return ResponseDto<string>.Fail(
                        ErrorCode.BadRequest,
                        "No paid payment to refund");
                }

                // Refund the payment
                var refundResult = await _paymentService.RefundPaymentAsync(
                    paidPayment.Id,
                    cancellationReason);

                if (!refundResult.IsSuccess)
                {
                    return ResponseDto<string>.Fail(
                        refundResult.ErrorCode,
                        "Refund failed");
                }

                return ResponseDto<string>.Success(
                    "Reservation cancelled and payment refunded successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<string>.Fail(
                    ErrorCode.ServerError,
                    $"Error: {ex.Message}");
            }
        }
    }
}
