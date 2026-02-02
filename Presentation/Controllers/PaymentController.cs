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

        /// <summary>
        /// Initiate payment for a reservation
        /// POST: api/payment/initiate
        /// </summary>
        [HttpPost("initiate")]
        public async Task<ActionResult<ResponseDto<PaymentResponseDto>>> InitiatePayment(
            [FromBody] InitiatePaymentRequestDto request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _paymentService.InitiatePaymentAsync(
                request.ReservationId,
                request.CustomerId,
                ipAddress);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Verify payment status
        /// POST: api/payment/verify
        /// </summary>
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

        /// <summary>
        /// Handle payment gateway webhook
        /// POST: api/payment/webhook
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<string>>> HandleWebhook([FromBody] string webhookData)
        {
            var result = await _paymentService.HandleWebhookAsync(webhookData);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get payment history for a reservation
        /// GET: api/payment/history/{reservationId}
        /// </summary>
        [HttpGet("history/{reservationId}")]
        public async Task<ActionResult<ResponseDto<List<PaymentResponseDto>>>> GetPaymentHistory(Guid reservationId)
        {
            var result = await _paymentService.GetPaymentHistoryAsync(reservationId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Refund a payment
        /// POST: api/payment/refund
        /// </summary>
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
