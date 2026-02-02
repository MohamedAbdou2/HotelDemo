# 🎓 Step-by-Step: How to Apply Payment to Reservations
# شرح خطوة بخطوة: كيفية تطبيق الدفع على الحجوزات

---

## 📋 Table of Contents
1. Understanding the Flow
2. Using the Payment Service
3. Complete Example
4. Testing
5. Troubleshooting

---

## 1️⃣ Understanding the Flow

### Current Situation (Before Payment)
```
User → Create Reservation → Reservation Created (Status: Pending)
```

### After Payment System Implementation
```
User → Create Reservation → Reservation Created (Pending)
    ↓
    → Initiate Payment → Payment Created (Pending)
    ↓
    → User Pays at Gateway → Gateway Confirms
    ↓
    → System Gets Confirmation → Payment Status = Paid
    ↓
    → Reservation Status = Confirmed ✅
```

---

## 2️⃣ Using the Payment Service

### Import the Service

In your Controller or Service class, add:

```csharp
using Application.Interfaces;
using Application.Dtos.Payment;

public class YourController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public YourController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    // Your methods below...
}
```

### Method 1: Initiate Payment (بدء الدفع)

```csharp
[HttpPost("pay-for-reservation")]
public async Task<IActionResult> PayForReservation(
    Guid reservationId, 
    Guid customerId)
{
    // Get user's IP address for security
    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
    
    // Call payment service
    var result = await _paymentService.InitiatePaymentAsync(
        reservationId: reservationId,
        customerId: customerId,
        ipAddress: ipAddress
    );

    if (!result.IsSuccess)
    {
        return BadRequest(new 
        { 
            error = result.Message,
            errorCode = result.ErrorCode 
        });
    }

    // Return payment ID to frontend
    return Ok(new 
    { 
        paymentId = result.Data.Id,
        amount = result.Data.Amount,
        message = "Payment initiated. Please proceed to payment gateway."
    });
}
```

### Method 2: Verify Payment (التحقق من الدفع)

After user completes payment at gateway:

```csharp
[HttpPost("confirm-payment")]
[AllowAnonymous] // Or keep it protected
public async Task<IActionResult> ConfirmPayment(
    [FromQuery] Guid paymentId,
    [FromQuery] string transactionId)
{
    var result = await _paymentService.VerifyPaymentAsync(
        paymentId: paymentId,
        transactionId: transactionId
    );

    if (!result.IsSuccess)
    {
        return BadRequest(new { error = result.Message });
    }

    return Ok(new 
    { 
        message = "Payment confirmed!",
        paymentStatus = result.Data.PaymentStatusId,
        reservationId = result.Data.ReservationId,
        completedAt = result.Data.CompletedAt
    });
}
```

### Method 3: Check Payment Status (فحص حالة الدفع)

```csharp
[HttpGet("reservation/{reservationId}/payment-status")]
public async Task<IActionResult> GetPaymentStatus(Guid reservationId)
{
    var result = await _paymentService.GetPaymentHistoryAsync(reservationId);

    if (!result.IsSuccess)
    {
        return BadRequest(new { error = result.Message });
    }

    // Get the latest payment
    var latestPayment = result.Data.FirstOrDefault();

    return Ok(new 
    { 
        isPaid = latestPayment?.IsSuccessful ?? false,
        paymentStatus = latestPayment?.PaymentStatusId,
        paymentHistory = result.Data
    });
}
```

### Method 4: Process Refund (معالجة الاسترجاع)

```csharp
[HttpPost("refund-payment")]
public async Task<IActionResult> RefundPayment(
    [FromQuery] Guid paymentId,
    [FromQuery] string reason)
{
    var result = await _paymentService.RefundPaymentAsync(
        paymentId: paymentId,
        refundReason: reason
    );

    if (!result.IsSuccess)
    {
        return BadRequest(new { error = result.Message });
    }

    return Ok(new 
    { 
        message = "Payment refunded successfully",
        refundedAt = result.Data.RefundedAt,
        refundAmount = result.Data.RefundAmount
    });
}
```

### Method 5: Handle Webhook (معالجة استدعاء البوابة)

```csharp
[HttpPost("webhook")]
[AllowAnonymous] // Important: No auth for webhooks!
public async Task<IActionResult> HandlePaymentWebhook([FromBody] string webhookData)
{
    // This is called by payment gateway after payment
    var result = await _paymentService.HandleWebhookAsync(webhookData);

    if (!result.IsSuccess)
    {
        return BadRequest(new { error = result.Message });
    }

    // Return 200 OK to acknowledge webhook receipt
    return Ok(new { message = "Webhook processed" });
}
```

---

## 3️⃣ Complete Example

### Full Controller Implementation

```csharp
using Application.Interfaces;
using Application.Dtos;
using Application.Dtos.Reservation;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationPaymentController : ControllerBase
    {
        private readonly IReservationServices _reservationService;
        private readonly IPaymentService _paymentService;

        public ReservationPaymentController(
            IReservationServices reservationService,
            IPaymentService paymentService)
        {
            _reservationService = reservationService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// 1. Create reservation and start payment
        /// </summary>
        [HttpPost("book-with-payment")]
        [Authorize]
        public async Task<IActionResult> BookRoomWithPayment(
            [FromBody] ReservationDto reservationDto,
            [FromQuery] Guid customerId)
        {
            try
            {
                // Create reservation
                var reservationResult = await _reservationService.CreateReservation(reservationDto);
                if (!reservationResult.IsSuccess)
                    return BadRequest(reservationResult);

                var reservationId = reservationResult.Data.Id;

                // Get user IP for fraud detection
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                // Initiate payment
                var paymentResult = await _paymentService.InitiatePaymentAsync(
                    reservationId,
                    customerId,
                    ipAddress
                );

                if (!paymentResult.IsSuccess)
                    return BadRequest(paymentResult);

                return Ok(new
                {
                    success = true,
                    message = "Reservation created. Proceed to payment.",
                    data = new
                    {
                        reservationId,
                        paymentId = paymentResult.Data.Id,
                        amount = paymentResult.Data.Amount,
                        status = paymentResult.Data.PaymentStatusId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// 2. Confirm payment after gateway redirect
        /// </summary>
        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmPayment(
            [FromQuery] Guid paymentId,
            [FromQuery] string transactionId)
        {
            var result = await _paymentService.VerifyPaymentAsync(paymentId, transactionId);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Message });

            return Ok(new
            {
                success = true,
                message = "✅ Payment confirmed successfully!",
                data = new
                {
                    paymentStatus = result.Data.PaymentStatusId,
                    completedAt = result.Data.CompletedAt
                }
            });
        }

        /// <summary>
        /// 3. Check payment status
        /// </summary>
        [HttpGet("status/{reservationId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(Guid reservationId)
        {
            var result = await _paymentService.GetPaymentHistoryAsync(reservationId);

            if (!result.IsSuccess)
                return BadRequest(result);

            var latestPayment = result.Data.FirstOrDefault();
            var isPaid = latestPayment?.IsSuccessful ?? false;

            return Ok(new
            {
                reservationId,
                isPaid,
                latestStatus = latestPayment?.PaymentStatusId,
                history = result.Data
            });
        }

        /// <summary>
        /// 4. Process refund
        /// </summary>
        [HttpPost("refund/{paymentId}")]
        [Authorize]
        public async Task<IActionResult> RefundPayment(
            Guid paymentId,
            [FromQuery] string reason = "Customer request")
        {
            var result = await _paymentService.RefundPaymentAsync(paymentId, reason);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(new
            {
                success = true,
                message = "🔄 Payment refunded successfully",
                refundedAt = result.Data.RefundedAt
            });
        }

        /// <summary>
        /// 5. Webhook endpoint (called by payment gateway)
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook([FromBody] string webhookData)
        {
            var result = await _paymentService.HandleWebhookAsync(webhookData);

            if (!result.IsSuccess)
                // Still return 200 to prevent gateway from retrying
                return Ok(new { error = result.Message });

            return Ok(new { success = true });
        }
    }
}
```

---

## 4️⃣ Testing

### Unit Test Example

```csharp
using Moq;
using Xunit;

public class PaymentServiceTests
{
    [Fact]
    public async Task InitiatePayment_WithValidReservation_ReturnsSuccess()
    {
        // Arrange
        var mockPaymentRepo = new Mock<IGenericRepository<Payment>>();
        var mockReservationRepo = new Mock<IGenericRepository<Reservation>>();
        var mockMapper = new Mock<IMapper>();

        var reservation = new Reservation { Id = Guid.NewGuid(), TotalPrice = 150 };
        mockReservationRepo.Setup(x => x.GetbyId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { reservation }.AsQueryable());

        var service = new PaymentService(mockPaymentRepo.Object, mockReservationRepo.Object, mockMapper.Object);

        // Act
        var result = await service.InitiatePaymentAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "192.168.1.1"
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
```

### Manual Testing with Curl

```bash
# 1. Initiate payment
curl -X POST http://localhost:5000/api/payment/initiate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "reservationId": "550e8400-e29b-41d4-a716-446655440000",
    "customerId": "550e8400-e29b-41d4-a716-446655440001"
  }'

# 2. Verify payment
curl -X POST "http://localhost:5000/api/payment/verify?paymentId=xxx&transactionId=TXN123456"

# 3. Get payment history
curl -X GET http://localhost:5000/api/payment/history/550e8400-e29b-41d4-a716-446655440000 \
  -H "Authorization: Bearer YOUR_TOKEN"

# 4. Refund
curl -X POST http://localhost:5000/api/payment/refund?paymentId=xxx&refundReason=test \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 5️⃣ Troubleshooting

### Issue 1: "Payment service not found" error

**Solution:** Ensure DependencyInjection.cs has:
```csharp
services.AddScoped<IPaymentService, PaymentService>();
```

### Issue 2: "Reservation not found" error

**Solution:** Check that reservationId exists in database:
```csharp
var reservation = await _reservationRepository.GetbyId(reservationId);
if (reservation.FirstOrDefault() == null)
    // Reservation doesn't exist
```

### Issue 3: "Payment already exists" error

**Solution:** Check if reservation is already paid:
```csharp
var result = await _paymentService.GetPaymentHistoryAsync(reservationId);
var isPaid = result.Data.Any(p => p.IsSuccessful);
```

### Issue 4: Database constraint violation

**Solution:** Ensure Amount > 0 and RefundAmount ≤ Amount

---

## 🎯 Summary

You now understand how to:
✅ Initiate payments  
✅ Verify payments  
✅ Get payment status  
✅ Process refunds  
✅ Handle webhooks  

**The payment system is ready to use!** 🚀
