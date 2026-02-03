using Application.Dtos;
using Application.Dtos.Payment;

namespace Application.Interfaces
{
    public interface IRefundService
    {
        Task<ResponseDto<RefundResponseDto>> InitiateRefundAsync(RefundRequestDto request);
        Task<ResponseDto<RefundResponseDto>> ProcessRefundAsync(Guid refundId);
        Task<ResponseDto<List<RefundResponseDto>>> GetRefundsByPaymentAsync(Guid paymentId);
        Task<ResponseDto<RefundResponseDto>> CancelRefundAsync(Guid refundId, string reason);
    }
}
