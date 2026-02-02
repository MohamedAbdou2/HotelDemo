using Application.Dtos;
using Application.Dtos.Payment;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Services.PaymentServices
{
    /// <summary>
    /// Service for handling payment operations
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<Reservation> reservationRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Initiate payment for a reservation
        /// </summary>
        public async Task<ResponseDto<PaymentResponseDto>> InitiatePaymentAsync(
            Guid reservationId,
            Guid customerId,
            string? ipAddress = null)
        {
            try
            {
                // 1. Get reservation and validate it exists
                var reservationQuery = await _reservationRepository.GetbyId(reservationId);
                var reservation = reservationQuery.FirstOrDefault();
                
                if (reservation == null)
                {
                    return ResponseDto<PaymentResponseDto>.Fail(
                        ErrorCode.NotFound,
                        "Reservation not found");
                }

                // 2. Check if reservation is already paid
                var existingPayments = await _paymentRepository.GetAll(
                    p => p.ReservationId == reservationId && p.PaymentStatusId == PaymentStatusCode.Paid);
                
                if (existingPayments.Any())
                {
                    return ResponseDto<PaymentResponseDto>.Fail(
                        ErrorCode.BadRequest,
                        "Reservation already paid");
                }

                // 3. Create new payment record
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    ReservationId = reservationId,
                    CustomerId = customerId,
                    Amount = reservation.TotalPrice,
                    PaymentMethodId = PaymentMethodCode.Stripe,
                    PaymentStatusId = PaymentStatusCode.Pending,
                    IpAddress = ipAddress,
                    WebhookVerified = false
                };

                // 4. Save payment record
                await _paymentRepository.Add(payment);

                var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
                return ResponseDto<PaymentResponseDto>.Success(
                    paymentDto,
                    "Payment initiated successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<PaymentResponseDto>.Fail(
                    ErrorCode.ServerError,
                    $"Error initiating payment: {ex.Message}");
            }
        }

        /// <summary>
        /// Verify payment status from payment gateway
        /// </summary>
        public async Task<ResponseDto<PaymentResponseDto>> VerifyPaymentAsync(
            Guid paymentId,
            string transactionId)
        {
            try
            {
                // 1. Get payment record
                var paymentQuery = await _paymentRepository.GetbyId(paymentId);
                var payment = paymentQuery.FirstOrDefault();
                
                if (payment == null)
                {
                    return ResponseDto<PaymentResponseDto>.Fail(
                        ErrorCode.NotFound,
                        "Payment not found");
                }

                // 2. Update transaction ID
                payment.TransactionId = transactionId;
                payment.PaymentStatusId = PaymentStatusCode.Paid;
                payment.CompletedAt = DateTime.UtcNow;
                payment.WebhookVerified = true;

                // 3. Update reservation status to Confirmed
                var reservationQuery = await _reservationRepository.GetbyId(payment.ReservationId);
                var reservation = reservationQuery.FirstOrDefault();
                
                if (reservation != null)
                {
                    reservation.ReservationStatusId = ReservationStatusCode.Confirmed;
                    reservation.ConfirmedAt = DateTime.UtcNow;
                    await _reservationRepository.Update(reservation);
                }

                await _paymentRepository.Update(payment);

                var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
                return ResponseDto<PaymentResponseDto>.Success(
                    paymentDto,
                    "Payment verified successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<PaymentResponseDto>.Fail(
                    ErrorCode.ServerError,
                    $"Error verifying payment: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle webhook callback from payment gateway
        /// </summary>
        public async Task<ResponseDto<string>> HandleWebhookAsync(string webhookData)
        {
            try
            {
                // Parse webhook data (example structure)
                var webhookJson = JsonSerializer.Deserialize<JsonElement>(webhookData);
                
                if (!webhookJson.TryGetProperty("transaction_id", out var transactionIdElement) ||
                    !webhookJson.TryGetProperty("amount_cents", out var amountElement) ||
                    !webhookJson.TryGetProperty("success", out var successElement))
                {
                    return ResponseDto<string>.Fail(
                        ErrorCode.BadRequest,
                        "Invalid webhook data structure");
                }

                var transactionId = transactionIdElement.GetString();
                var isSuccess = successElement.GetBoolean();

                // Find payment by transaction ID
                var paymentsQuery = await _paymentRepository.GetAll(
                    p => p.TransactionId == transactionId);
                var payment = paymentsQuery.FirstOrDefault();

                if (payment == null)
                {
                    return ResponseDto<string>.Fail(
                        ErrorCode.NotFound,
                        "Payment not found");
                }

                // Update payment based on webhook
                if (isSuccess)
                {
                    payment.PaymentStatusId = PaymentStatusCode.Paid;
                    payment.CompletedAt = DateTime.UtcNow;
                    
                    // Update reservation
                    var reservationQuery = await _reservationRepository.GetbyId(payment.ReservationId);
                    var reservation = reservationQuery.FirstOrDefault();
                    
                    if (reservation != null)
                    {
                        reservation.ReservationStatusId = ReservationStatusCode.Confirmed;
                        reservation.ConfirmedAt = DateTime.UtcNow;
                        await _reservationRepository.Update(reservation);
                    }
                }
                else
                {
                    payment.PaymentStatusId = PaymentStatusCode.Failed;
                    payment.FailedAt = DateTime.UtcNow;
                    payment.FailureReason = webhookJson.TryGetProperty("error_message", out var errorMsg) 
                        ? errorMsg.GetString() 
                        : "Payment failed";
                }

                payment.WebhookVerified = true;
                payment.GatewayResponse = webhookData;

                await _paymentRepository.Update(payment);

                return ResponseDto<string>.Success(
                    "Webhook processed successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<string>.Fail(
                    ErrorCode.ServerError,
                    $"Error processing webhook: {ex.Message}");
            }
        }

        /// <summary>
        /// Get payment history for a reservation
        /// </summary>
        public async Task<ResponseDto<List<PaymentResponseDto>>> GetPaymentHistoryAsync(Guid reservationId)
        {
            try
            {
                var paymentsQuery = await _paymentRepository.GetAll(
                    p => p.ReservationId == reservationId);

                var payments = paymentsQuery.OrderByDescending(p => p.CreatedAt).ToList();
                var paymentDtos = _mapper.Map<List<PaymentResponseDto>>(payments);
                
                return ResponseDto<List<PaymentResponseDto>>.Success(paymentDtos);
            }
            catch (Exception ex)
            {
                return ResponseDto<List<PaymentResponseDto>>.Fail(
                    ErrorCode.ServerError,
                    $"Error fetching payment history: {ex.Message}");
            }
        }

        /// <summary>
        /// Refund a successful payment
        /// </summary>
        public async Task<ResponseDto<PaymentResponseDto>> RefundPaymentAsync(
            Guid paymentId,
            string refundReason)
        {
            try
            {
                var paymentQuery = await _paymentRepository.GetbyId(paymentId);
                var payment = paymentQuery.FirstOrDefault();
                
                if (payment == null)
                {
                    return ResponseDto<PaymentResponseDto>.Fail(
                        ErrorCode.NotFound,
                        "Payment not found");
                }

                if (payment.PaymentStatusId != PaymentStatusCode.Paid)
                {
                    return ResponseDto<PaymentResponseDto>.Fail(
                        ErrorCode.BadRequest,
                        "Can only refund paid payments");
                }

                // Update payment
                payment.PaymentStatusId = PaymentStatusCode.Refunded;
                payment.RefundedAt = DateTime.UtcNow;
                payment.RefundReason = refundReason;
                payment.RefundAmount = payment.Amount;

                await _paymentRepository.Update(payment);

                var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
                return ResponseDto<PaymentResponseDto>.Success(
                    paymentDto,
                    "Payment refunded successfully");
            }
            catch (Exception ex)
            {
                return ResponseDto<PaymentResponseDto>.Fail(
                    ErrorCode.ServerError,
                    $"Error refunding payment: {ex.Message}");
            }
        }
    }
}
