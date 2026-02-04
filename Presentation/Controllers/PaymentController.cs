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

        [HttpGet("success")]
        [AllowAnonymous]
        public async Task<ActionResult> PaymentSuccess([FromQuery] Guid paymentId)
        {
            var result = await _paymentService.HandleStripeSuccessAsync(paymentId);

            if (!result.IsSuccess)
                return Redirect($"/payment-failed?error={result.Message}");

            return Redirect($"/payment-success?paymentId={paymentId}");
        }

        [HttpGet("cancel")]
        [AllowAnonymous]
        public async Task<ActionResult> PaymentCancel([FromQuery] Guid paymentId)
        {
            await _paymentService.HandleStripeCancelAsync(paymentId);
            return Redirect($"/payment-cancelled?paymentId={paymentId}");
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<ActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            var result = await _paymentService.HandleStripeWebhookAsync(json, stripeSignature);

            if (!result.IsSuccess)
                return BadRequest();

            return Ok();
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