using Application.Dtos;
using Application.Dtos.Payment;
using Application.Interfaces;
using Application.Helper;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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

            var reservation = await GetReservationById(reservationId);
            if (reservation == null)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.NotFound, "Reservation not found");

            var isAlreadyPaid = await IsReservationAlreadyPaid(reservationId);
            if (isAlreadyPaid)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.BadRequest, "Reservation already paid");

            var payment = await CreatePaymentEntity(reservation, ipAddress);
            var paymentDto = _mapper.Map<PaymentResponseDto>(payment);

            return ResponseDto<PaymentResponseDto>.Success(paymentDto, "Payment initiated successfully");
        }

        private async Task<Reservation> GetReservationById(Guid reservationId)
        {
            
            return await _reservationRepository.GetbyId(reservationId).FirstOrDefaultAsync();
        }

        private async Task<bool> IsReservationAlreadyPaid(Guid reservationId)
        {
            var existingPayments = await _paymentRepository.GetAll(
                p => p.ReservationId == reservationId && p.PaymentStatusId == PaymentStatusCode.Paid).AnyAsync();

            return existingPayments;
        }

        private async Task<Payment> CreatePaymentEntity(Reservation reservation, string ipAddress)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                ReservationId = reservation.Id,
                CustomerId = reservation.CustomerId,
                CreatedById = reservation.CreatedById,
                Amount = reservation.TotalPrice,
                PaymentMethodId = PaymentMethodCode.Stripe,
                PaymentStatusId = PaymentStatusCode.Pending,
                IpAddress = ipAddress,
                WebhookVerified = false
            };

            var paymentUrl = await _stripeService.CreatePaymentSessionAsync(payment.Id, payment.Amount);
            payment.PaymentUrl = paymentUrl;

            await _paymentRepository.Add(payment);
            return payment;
        }

        public async Task<ResponseDto<PaymentResponseDto>> HandleStripeSuccessAsync(Guid paymentId)
        {
            var payment = await GetPaymentById(paymentId);
            if (payment == null)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.NotFound, "Payment not found");

            if (payment.PaymentStatusId == PaymentStatusCode.Paid)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.BadRequest, "Payment already processed");

            var (isSuccessful, sessionId, paymentIntentId) = await _stripeService.GetSessionStatusAsync(paymentId);
            if (!isSuccessful)
                return ResponseDto<PaymentResponseDto>.Fail(ErrorCode.BadRequest, "Payment not completed on Stripe");

            await CompletePayment(payment, sessionId, paymentIntentId);
            await ConfirmReservation(payment.ReservationId);
            var CustomerEmail = await GetCustomerEmail(payment.Id);
            if(CustomerEmail != null) // Check if email is not null
            {
                await MailSender.SendAsync(CustomerEmail, "Reservation Confirmed", "Your reservation is confirmed");
            }
            var paymentDto = _mapper.Map<PaymentResponseDto>(payment);
            return ResponseDto<PaymentResponseDto>.Success(paymentDto, "Payment verified and confirmed successfully");
        }

        private async Task<string> GetCustomerEmail(Guid paymentId)
        {
            var email = await _paymentRepository.GetAll(r => r.Id == paymentId)
                .Include(x => x.Customer).ThenInclude(x => x.User).Select(x => x.Customer.User.Email).FirstOrDefaultAsync();
            return email;
        }

        private async Task<Payment> GetPaymentById(Guid paymentId)
        {
            return _paymentRepository.GetbyId(paymentId).FirstOrDefault();
        }

        private async Task CompletePayment(Payment payment, string sessionId, string paymentIntentId)
        {
            payment.TransactionId = paymentIntentId;
            payment.PaymentStatusId = PaymentStatusCode.Paid;
            payment.CompletedAt = DateTime.UtcNow;
            payment.GatewayResponse = $"{{\"session_id\": \"{sessionId}\", \"payment_intent_id\": \"{paymentIntentId}\"}}";

            await _paymentRepository.Update(payment);
        }

        private async Task ConfirmReservation(Guid reservationId)
        {
            var reservation = await GetReservationById(reservationId);
            if (reservation != null)
            {
                reservation.ReservationStatusId = ReservationStatusCode.Confirmed;
                reservation.ConfirmedAt = DateTime.UtcNow;
                await _reservationRepository.Update(reservation);
            }
        }

        public async Task<ResponseDto<string>> HandleStripeCancelAsync(Guid paymentId)
        {
            var payment = await GetPaymentById(paymentId);
            if (payment == null)
                return ResponseDto<string>.Fail(ErrorCode.NotFound, "Payment not found");

            if (payment.PaymentStatusId != PaymentStatusCode.Pending)
                return ResponseDto<string>.Success("Payment already processed");

            await MarkPaymentAsFailed(payment, "Payment cancelled by user");
            return ResponseDto<string>.Success("Payment cancelled");
        }

        private async Task MarkPaymentAsFailed(Payment payment, string reason)
        {
            payment.PaymentStatusId = PaymentStatusCode.Failed;
            payment.FailedAt = DateTime.UtcNow;
            payment.FailureReason = reason;
            await _paymentRepository.Update(payment);
        }

        public async Task<ResponseDto<string>> HandleStripeWebhookAsync(string json, string stripeSignature)
        {
            var webhookResult = await _stripeService.ProcessWebhookAsync(json, stripeSignature);

            if (!webhookResult.isValid)
                return ResponseDto<string>.Fail(ErrorCode.BadRequest, "Invalid webhook signature");

            if (webhookResult.eventType == "checkout.session.completed" && webhookResult.paymentId.HasValue)
            {
                await ProcessSuccessfulWebhookPayment(webhookResult.paymentId.Value, webhookResult.paymentIntentId, json);
            }

            return ResponseDto<string>.Success("Webhook processed successfully");
        }

        private async Task ProcessSuccessfulWebhookPayment(Guid paymentId, string paymentIntentId, string gatewayResponse)
        {
            var payment = await GetPaymentById(paymentId);
            if (payment == null || payment.PaymentStatusId != PaymentStatusCode.Pending)
                return;

            payment.TransactionId = paymentIntentId;
            payment.PaymentStatusId = PaymentStatusCode.Paid;
            payment.CompletedAt = DateTime.UtcNow;
            payment.WebhookVerified = true;
            payment.GatewayResponse = gatewayResponse;

            await _paymentRepository.Update(payment);
            await ConfirmReservation(payment.ReservationId);
        }

        public async Task<ResponseDto<List<PaymentResponseDto>>> GetPaymentHistoryAsync(Guid reservationId)
        {
            var payments = _paymentRepository.GetAll(p => p.ReservationId == reservationId).OrderByDescending(p => p.CreatedAt).ToList();
            var paymentDtos = _mapper.Map<List<PaymentResponseDto>>(payments);

            return ResponseDto<List<PaymentResponseDto>>.Success(paymentDtos);
        }


    }
}
