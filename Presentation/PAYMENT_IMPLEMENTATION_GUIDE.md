# شرح نظام الدفع في تطبيق الفندق
# Payment System Implementation Guide

## 📋 نظرة عامة | Overview

I've implemented a complete payment system for your hotel reservation application. Here's how it works:

---

## 🏗️ Architecture (البنية المعمارية)

### Layers:
1. **Domain Layer** - Payment & PaymentStatus models
2. **Application Layer** - Business logic (PaymentService, DTOs)
3. **Presentation Layer** - API Controllers (PaymentController)
4. **Infrastructure Layer** - Database configuration (PaymentConfiguration)

---

## 🔄 Payment Flow (تدفق الدفع)

```
1. User Creates Reservation
   ↓
2. System Creates Reservation (Status: Pending)
   ↓
3. User Initiates Payment
   ↓
4. System Creates Payment Record (Status: Pending)
   ↓
5. Payment Gateway Processes Payment
   ↓
6. Gateway Sends Webhook Confirmation
   ↓
7. System Updates Payment Status to Paid
   ↓
8. System Updates Reservation Status to Confirmed
```

---

## 📁 Files Created

### 1. **Interfaces** (`IPaymentService.cs`)
Defines contract for payment operations:
- ✅ `InitiatePaymentAsync` - Start payment process
- ✅ `VerifyPaymentAsync` - Confirm payment
- ✅ `HandleWebhookAsync` - Process gateway callback
- ✅ `GetPaymentHistoryAsync` - Retrieve payment records
- ✅ `RefundPaymentAsync` - Process refunds

### 2. **DTOs** (Data Transfer Objects)
- **`PaymentResponseDto`** - Response to client with payment details
- **`InitiatePaymentRequestDto`** - Request to start payment

### 3. **Service** (`PaymentService.cs`)
Implements business logic:
```csharp
// Example: Initiate payment
var result = await _paymentService.InitiatePaymentAsync(
    reservationId: reservationId,
    customerId: customerId,
    ipAddress: userIpAddress
);
```

### 4. **Controller** (`PaymentController.cs`)
API endpoints:
```
POST   /api/payment/initiate          - Start payment
POST   /api/payment/verify            - Verify payment status
POST   /api/payment/webhook           - Handle gateway webhook
GET    /api/payment/history/{id}      - Get payment history
POST   /api/payment/refund            - Process refund
```

### 5. **AutoMapper Profile**
Configures mapping between entities and DTOs

---

## 🔌 API Usage Examples

### 1. Initiate Payment (بدء الدفع)

**Request:**
```json
POST /api/payment/initiate
{
    "reservationId": "550e8400-e29b-41d4-a716-446655440000",
    "customerId": "550e8400-e29b-41d4-a716-446655440001",
    "paymentMethodId": 1  // Stripe
}
```

**Response:**
```json
{
    "isSuccess": true,
    "message": "Payment initiated successfully",
    "data": {
        "id": "550e8400-e29b-41d4-a716-446655440002",
        "reservationId": "550e8400-e29b-41d4-a716-446655440000",
        "amount": 150.00,
        "paymentStatusId": 1,  // Pending
        "createdAt": "2024-01-15T10:30:00Z"
    }
}
```

### 2. Verify Payment (التحقق من الدفع)

**Request:**
```
POST /api/payment/verify?paymentId=550e8400-e29b-41d4-a716-446655440002&transactionId=TXN123456
```

**Response:**
```json
{
    "isSuccess": true,
    "data": {
        "paymentStatusId": 2,  // Paid
        "completedAt": "2024-01-15T10:35:00Z"
    }
}
```

### 3. Handle Webhook (معالجة رد الاتصال)

**Request from Payment Gateway:**
```json
POST /api/payment/webhook
{
    "transaction_id": "TXN123456",
    "amount_cents": 15000,
    "success": true,
    "error_message": null
}
```

### 4. Get Payment History (الحصول على سجل الدفع)

**Request:**
```
GET /api/payment/history/550e8400-e29b-41d4-a716-446655440000
```

**Response:**
```json
{
    "isSuccess": true,
    "data": [
        {
            "id": "550e8400-e29b-41d4-a716-446655440002",
            "amount": 150.00,
            "paymentStatusId": 2,  // Paid
            "transactionId": "TXN123456",
            "completedAt": "2024-01-15T10:35:00Z",
            "isSuccessful": true
        }
    ]
}
```

### 5. Refund Payment (استرجاع الدفع)

**Request:**
```
POST /api/payment/refund?paymentId=550e8400-e29b-41d4-a716-446655440002&refundReason=Customer requested refund
```

---

## 📊 Database Schema (الجدول في قاعدة البيانات)

The `Payments` table includes:

| Column | Type | Purpose |
|--------|------|---------|
| `Id` | GUID | Primary key |
| `ReservationId` | GUID | Links to reservation |
| `CustomerId` | GUID | Customer who paid |
| `Amount` | Decimal(18,2) | Payment amount |
| `PaymentMethodId` | Int | Payment method (Stripe=1, Cash=2) |
| `PaymentStatusId` | Int | Status (Pending=1, Paid=2, Failed=3, Refunded=4) |
| `TransactionId` | String(200) | Gateway transaction ID |
| `GatewayResponse` | Text | Full response from gateway |
| `CompletedAt` | DateTime | When payment succeeded |
| `FailedAt` | DateTime | When payment failed |
| `RefundedAt` | DateTime | When refund processed |
| `RowVersion` | Binary | Concurrency control |

---

## 🔐 Payment Constraints (القيود الأمنية)

```sql
-- Amount must be positive
CHECK ([Amount] > 0)

-- Refund cannot exceed original payment
CHECK ([RefundAmount] IS NULL OR [RefundAmount] <= [Amount])
```

---

## 🗂️ File Locations

```
..\Application\
├── Interfaces\
│   └── IPaymentService.cs
├── Dtos\Payment\
│   ├── PaymentResponseDto.cs
│   └── InitiatePaymentRequestDto.cs
├── Services\PaymentServices\
│   └── PaymentService.cs
└── MappingProfiles\Payment\
    └── PaymentProfile.cs

Presentation\Controllers\
└── PaymentController.cs
```

---

## 🚀 Next Steps (الخطوات التالية)

1. **Integrate Payment Gateway** (ربط بوابة الدفع)
   - Implement Paymob / Stripe API integration
   - Add configuration in `appsettings.json`

2. **Frontend Implementation**
   - Create payment page UI
   - Call `/api/payment/initiate` endpoint
   - Redirect to payment gateway
   - Handle callback/webhook response

3. **Add Validators**
   - Create `InitiatePaymentRequestDtoValidator`
   - Add business rule validations

4. **Add Logging**
   - Log all payment attempts
   - Track webhook receipts

5. **Security Measures**
   - Verify webhook signatures from gateway
   - Add rate limiting to payment endpoints
   - Use HTTPS for all payment requests

---

## ⚙️ Configuration in DependencyInjection

The service is already registered in `DependencyInjection.cs`:

```csharp
services.AddScoped<IPaymentService, PaymentService>();
```

This allows you to inject it into controllers:

```csharp
[HttpPost("process")]
public async Task<IActionResult> ProcessPayment([FromServices] IPaymentService paymentService)
{
    var result = await paymentService.InitiatePaymentAsync(...);
    return Ok(result);
}
```

---

## 💡 Key Points (النقاط المهمة)

✅ **What I've Created:**
- Complete payment service with 5 main operations
- Database configuration with constraints
- API controller with all endpoints
- AutoMapper integration
- Error handling with proper ErrorCodes

✅ **Why This Architecture:**
- **Separation of Concerns** - Each layer has single responsibility
- **Scalability** - Easy to add new payment methods
- **Maintainability** - Clear contract through interfaces
- **Testing** - Service can be mocked in unit tests
- **Security** - Data validation and constraint checks

---

## 📝 Common Scenarios

### Scenario 1: User Pays for Reservation
```csharp
// 1. Create reservation (already done)
var reservation = await _reservationService.CreateReservation(dto);

// 2. Initiate payment
var paymentResult = await _paymentService.InitiatePaymentAsync(
    reservation.Data.Id,
    customerId
);

// 3. Redirect user to payment gateway with paymentResult.Data.Id

// 4. After user completes payment, gateway calls webhook
// 5. System automatically updates payment & reservation status
```

### Scenario 2: Payment Fails
```csharp
// User returns from failed payment
// Check payment status
var payments = await _paymentService.GetPaymentHistoryAsync(reservationId);

if (payments.Data.Last().IsFailed)
{
    // Show error message to user
    // Allow retry with new payment initiation
}
```

### Scenario 3: Refund Request
```csharp
// Refund successful payment
var refundResult = await _paymentService.RefundPaymentAsync(
    paymentId,
    "Customer requested cancellation"
);
```

---

## ❓ Questions?

- Check `PaymentConfiguration.cs` for database constraints
- Check `Payment.cs` model for all properties
- Check `PaymentStatusCode` enum for status values
- Check `PaymentMethodCode` enum for payment methods

---

**Total Files Created: 6**
- 1 Interface
- 2 DTOs
- 1 Service Implementation
- 1 Controller
- 1 AutoMapper Profile

**All code follows your project's conventions and is ready to use!** ✨
