using Application.Dtos;
using Application.Dtos.Payment;
using Application.Interfaces;
using Application.Helper;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using System.Text.Json;

namespace Application.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Reservation> _reservationRepository;
        private readonly IStripePaymentService _stripeService;
        private readonly CurrentUser _currentUser;
        private readonly IMapper _mapper;

        public PaymentService(
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<Reservation> reservationRepository,
            IStripePaymentService stripeService,
            CurrentUser currentUser,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
            _stripeService = stripeService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<ResponseDto<PaymentResponseDto>> InitiatePaymentAsync(
            Guid reservationId,
            string? ipAddress = null)
        {
            var currentUserId = _currentUser.GetUserId();
            if (!currentUserId.HasValue)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.BadRequest, "User not authenticated");

            var reservationQuery = await _reservationRepository.GetbyId(reservationId);
            var reservation = reservationQuery.FirstOrDefault();

            if (reservation == null)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.NotFound, "Reservation not found");

            var existingPayments = await _paymentRepository.GetAll(
                p => p.ReservationId == reservationId && p.PaymentStatusId == PaymentStatusCode.Paid);

            if (existingPayments.Any())
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.BadRequest, "Reservation already paid");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                ReservationId = reservationId,
                CustomerId = currentUserId.Value,
                Amount = reservation.TotalPrice,
                PaymentMethodId = PaymentMethodCode.Stripe,
                PaymentStatusId = PaymentStatusCode.Pending,
                IpAddress = ipAddress,
                WebhookVerified = false
            };

            var paymentUrl = await _stripeService.CreatePaymentSessionAsync(payment.Id, payment.Amount);
            payment.PaymentUrl = paymentUrl;

            await _paymentRepository.Add(payment);

            var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
            return ResponseDto<PaymentResponseDto>.Success(paymentDto, "Payment initiated successfully");
        }

        public async Task<ResponseDto<PaymentResponseDto>> VerifyPaymentAsync(Guid paymentId, string transactionId)
        {
            var paymentQuery = await _paymentRepository.GetbyId(paymentId);
            var payment = paymentQuery.FirstOrDefault();

            if (payment == null)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.NotFound, "Payment not found");

            payment.TransactionId = transactionId;
            payment.PaymentStatusId = PaymentStatusCode.Paid;
            payment.CompletedAt = DateTime.UtcNow;
            payment.WebhookVerified = true;

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
            return ResponseDto<PaymentResponseDto>.Success(paymentDto, "Payment verified successfully");
        }

        public async Task<ResponseDto<string>> HandleWebhookAsync(string webhookData)
        {
            var webhookJson = JsonSerializer.Deserialize<JsonElement>(webhookData);

            if (!webhookJson.TryGetProperty("transaction_id", out var transactionIdElement) ||
                !webhookJson.TryGetProperty("success", out var successElement))
                return ResponseDto<string>.Fail(ErrorCode.BadRequest, "Invalid webhook data");

            var transactionId = transactionIdElement.GetString();
            var isSuccess = successElement.GetBoolean();

            var paymentsQuery = await _paymentRepository.GetAll(p => p.TransactionId == transactionId);
            var payment = paymentsQuery.FirstOrDefault();

            if (payment == null)
                return ResponseDto<string>.Fail(ErrorCode.NotFound, "Payment not found");

            if (isSuccess)
            {
                payment.PaymentStatusId = PaymentStatusCode.Paid;
                payment.CompletedAt = DateTime.UtcNow;

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

            return ResponseDto<string>.Success("Webhook processed successfully");
        }

        public async Task<ResponseDto<List<PaymentResponseDto>>> GetPaymentHistoryAsync(Guid reservationId)
        {
            var paymentsQuery = await _paymentRepository.GetAll(p => p.ReservationId == reservationId);
            var payments = paymentsQuery.OrderByDescending(p => p.CreatedAt).ToList();
            var paymentDtos = _mapper.Map<List<PaymentResponseDto>>(payments);

            return ResponseDto<List<PaymentResponseDto>>.Success(paymentDtos);
        }

        public async Task<ResponseDto<PaymentResponseDto>> RefundPaymentAsync(Guid paymentId, string refundReason)
        {
            var paymentQuery = await _paymentRepository.GetbyId(paymentId);
            var payment = paymentQuery.FirstOrDefault();

            if (payment == null)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.NotFound, "Payment not found");

            if (payment.PaymentStatusId != PaymentStatusCode.Paid)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.BadRequest, "Can only refund paid payments");

            payment.PaymentStatusId = PaymentStatusCode.Refunded;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundReason = refundReason;
            payment.RefundAmount = payment.Amount;

            await _paymentRepository.Update(payment);

            var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
            return ResponseDto<PaymentResponseDto>.Success(paymentDto, "Payment refunded successfully");
        }
    }
}
