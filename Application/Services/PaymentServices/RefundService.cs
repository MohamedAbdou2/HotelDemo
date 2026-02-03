using Application.Dtos;
using Application.Dtos.Payment;
using Application.Helper;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Domain.Repositories;

namespace Application.Services.PaymentServices
{
    public class RefundService : IRefundService
    {
        private readonly IGenericRepository<Refund> _refundRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IStripePaymentService _stripeService;
        private readonly CurrentUser _currentUser;
        private readonly IMapper _mapper;

        public RefundService(
            IGenericRepository<Refund> refundRepository,
            IGenericRepository<Payment> paymentRepository,
            IStripePaymentService stripeService,
            CurrentUser currentUser,
            IMapper mapper)
        {
            _refundRepository = refundRepository;
            _paymentRepository = paymentRepository;
            _stripeService = stripeService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<ResponseDto<RefundResponseDto>> InitiateRefundAsync(RefundRequestDto request)
        {
            var currentUserId = _currentUser.GetUserId();
            if (!currentUserId.HasValue)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.BadRequest, "User not authenticated");

            var paymentQuery = await _paymentRepository.GetbyId(request.PaymentId);
            var payment = paymentQuery.FirstOrDefault();

            if (payment == null)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.NotFound, "Payment not found");

            if (payment.PaymentStatusId != PaymentStatusCode.Paid)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.BadRequest, "Can only refund paid payments");

            if (request.Amount <= 0 || request.Amount > payment.RefundableAmount)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.BadRequest, 
                    $"Invalid refund amount. Refundable amount: {payment.RefundableAmount}");

            var refund = new Refund
            {
                Id = Guid.NewGuid(),
                PaymentId = request.PaymentId,
                Amount = request.Amount,
                Reason = request.Reason,
                RefundStatusId = RefundStatusCode.Pending,
                ProcessedByUserId = currentUserId.Value
            };

            await _refundRepository.Add(refund);

            var refundDto = _mapper.Map<RefundResponseDto>(refund);
            return ResponseDto<RefundResponseDto>.Success(refundDto, "Refund initiated successfully");
        }

        public async Task<ResponseDto<RefundResponseDto>> ProcessRefundAsync(Guid refundId)
        {
            var refundQuery = await _refundRepository.GetbyId(refundId);
            var refund = refundQuery.FirstOrDefault();

            if (refund == null)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.NotFound, "Refund not found");

            if (refund.RefundStatusId != RefundStatusCode.Pending)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.BadRequest, "Refund already processed");

            var paymentQuery = await _paymentRepository.GetbyId(refund.PaymentId);
            var payment = paymentQuery.FirstOrDefault();

            if (payment?.TransactionId == null)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.BadRequest, "Payment transaction ID not found");

            refund.RefundStatusId = RefundStatusCode.Processing;
            await _refundRepository.Update(refund);

            var success = await _stripeService.ProcessRefundAsync(payment.TransactionId, refund.Amount);

            if (success)
            {
                refund.RefundStatusId = RefundStatusCode.Completed;
                refund.ProcessedAt = DateTime.UtcNow;
                
                if (payment.RefundableAmount == 0)
                    payment.PaymentStatusId = PaymentStatusCode.Refunded;
            }
            else
            {
                refund.RefundStatusId = RefundStatusCode.Failed;
                refund.FailedAt = DateTime.UtcNow;
                refund.FailureReason = "Gateway refund failed";
            }

            await _refundRepository.Update(refund);

            var refundDto = _mapper.Map<RefundResponseDto>(refund);
            return ResponseDto<RefundResponseDto>.Success(refundDto, 
                success ? "Refund processed successfully" : "Refund failed");
        }

        public async Task<ResponseDto<List<RefundResponseDto>>> GetRefundsByPaymentAsync(Guid paymentId)
        {
            var refundsQuery = await _refundRepository.GetAll(r => r.PaymentId == paymentId);
            var refunds = refundsQuery.OrderByDescending(r => r.CreatedAt).ToList();
            var refundDtos = _mapper.Map<List<RefundResponseDto>>(refunds);

            return ResponseDto<List<RefundResponseDto>>.Success(refundDtos);
        }

        public async Task<ResponseDto<RefundResponseDto>> CancelRefundAsync(Guid refundId, string reason)
        {
            var refundQuery = await _refundRepository.GetbyId(refundId);
            var refund = refundQuery.FirstOrDefault();

            if (refund == null)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.NotFound, "Refund not found");

            if (refund.RefundStatusId != RefundStatusCode.Pending)
                return ResponseDto<RefundResponseDto>.Fail(ErrorCode.BadRequest, "Can only cancel pending refunds");

            refund.RefundStatusId = RefundStatusCode.Cancelled;
            refund.FailureReason = reason;

            await _refundRepository.Update(refund);

            var refundDto = _mapper.Map<RefundResponseDto>(refund);
            return ResponseDto<RefundResponseDto>.Success(refundDto, "Refund cancelled successfully");
        }
    }
}
