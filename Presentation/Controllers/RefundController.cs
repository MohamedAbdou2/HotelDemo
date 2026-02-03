using Application.Dtos;
using Application.Dtos.Payment;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpPost("initiate")]
        public async Task<ActionResult<ResponseDto<RefundResponseDto>>> InitiateRefund(
            [FromBody] RefundRequestDto request)
        {
            var result = await _refundService.InitiateRefundAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("process/{refundId}")]
        public async Task<ActionResult<ResponseDto<RefundResponseDto>>> ProcessRefund(Guid refundId)
        {
            var result = await _refundService.ProcessRefundAsync(refundId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("payment/{paymentId}")]
        public async Task<ActionResult<ResponseDto<List<RefundResponseDto>>>> GetRefundsByPayment(Guid paymentId)
        {
            var result = await _refundService.GetRefundsByPaymentAsync(paymentId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("cancel/{refundId}")]
        public async Task<ActionResult<ResponseDto<RefundResponseDto>>> CancelRefund(
            Guid refundId,
            [FromQuery] string reason)
        {
            var result = await _refundService.CancelRefundAsync(refundId, reason);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
