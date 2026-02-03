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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("initiate")]
        public async Task<ActionResult<ResponseDto<PaymentResponseDto>>> InitiatePayment(
            [FromBody] InitiatePaymentRequestDto request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _paymentService.InitiatePaymentAsync(request.ReservationId, ipAddress);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("verify")]
        public async Task<ActionResult<ResponseDto<PaymentResponseDto>>> VerifyPayment(
            [FromQuery] Guid paymentId,
            [FromQuery] string transactionId)
        {
            var result = await _paymentService.VerifyPaymentAsync(paymentId, transactionId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<string>>> HandleWebhook([FromBody] string webhookData)
        {
            var result = await _paymentService.HandleWebhookAsync(webhookData);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("history/{reservationId}")]
        public async Task<ActionResult<ResponseDto<List<PaymentResponseDto>>>> GetPaymentHistory(Guid reservationId)
        {
            var result = await _paymentService.GetPaymentHistoryAsync(reservationId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("refund")]
        public async Task<ActionResult<ResponseDto<PaymentResponseDto>>> RefundPayment(
            [FromQuery] Guid paymentId,
            [FromQuery] string refundReason)
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId, refundReason);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}