# 📊 Payment System Architecture & Flow Diagrams

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                        │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │           PaymentController.cs                              │ │
│  │  • POST /initiate          - Start payment                 │ │
│  │  • POST /verify            - Verify payment                │ │
│  │  • GET  /history           - Payment history               │ │
│  │  • POST /refund            - Process refund                │ │
│  │  • POST /webhook           - Handle gateway callback       │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                            │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │         IPaymentService (Interface)                         │ │
│  │  • InitiatePaymentAsync()                                  │ │
│  │  • VerifyPaymentAsync()                                    │ │
│  │  • HandleWebhookAsync()                                    │ │
│  │  • GetPaymentHistoryAsync()                                │ │
│  │  • RefundPaymentAsync()                                    │ │
│  └────────────────────────────────────────────────────────────┘ │
│                               ↓                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │         PaymentService (Implementation)                     │ │
│  │  • Business logic                                          │ │
│  │  • Error handling                                          │ │
│  │  • Status management                                       │ │
│  └────────────────────────────────────────────────────────────┘ │
│                               ↓                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │         DTOs (Data Transfer Objects)                        │ │
│  │  • PaymentResponseDto                                      │ │
│  │  • InitiatePaymentRequestDto                               │ │
│  │  • PaymentProfile (AutoMapper)                             │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                     DOMAIN LAYER                                 │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │        Payment.cs (Entity Model)                            │ │
│  │  • Id, ReservationId, CustomerId                           │ │
│  │  • Amount, PaymentMethodId, PaymentStatusId                │ │
│  │  • TransactionId, GatewayResponse                          │ │
│  │  • Timestamps, Refund info, Status flags                  │ │
│  └────────────────────────────────────────────────────────────┘ │
│                               ↓                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │        Enums                                                │ │
│  │  • PaymentStatusCode (Pending, Paid, Failed, Refunded)    │ │
│  │  • PaymentMethodCode (Stripe, Cash)                        │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                          │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  PaymentConfiguration.cs (EF Core Configuration)            │ │
│  │  • Table mapping                                           │ │
│  │  • Constraints & Indexes                                   │ │
│  │  • Relationships                                           │ │
│  │  • Concurrency control                                     │ │
│  └────────────────────────────────────────────────────────────┘ │
│                               ↓                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  GenericRepository<T>                                       │ │
│  │  • Add(), Update(), GetAll(), GetbyId()                    │ │
│  └────────────────────────────────────────────────────────────┘ │
│                               ↓                                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  ApplicationDbContext                                       │ │
│  │  • DbSet<Payment>                                          │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                               ↓
┌─────────────────────────────────────────────────────────────────┐
│                    DATABASE LAYER (SQL Server)                  │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │  Payments Table                                             │ │
│  │  • Primary Key: Id                                         │ │
│  │  • Foreign Keys: ReservationId, CustomerId                 │ │
│  │  • 7 Indexes for performance                               │ │
│  │  • 2 Check Constraints for data integrity                  │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Payment Flow Diagram

```
START
  │
  ├─── User Creates Reservation
  │    └─→ Reservation.Status = Pending
  │
  └─── User Initiates Payment
       │
       ├─→ POST /api/payment/initiate
       │   │
       │   ├─→ Service: InitiatePaymentAsync()
       │   │   │
       │   │   ├─→ Validate reservation exists
       │   │   │
       │   │   ├─→ Check not already paid
       │   │   │
       │   │   ├─→ Create Payment record
       │   │   │   └─→ Payment.Status = Pending
       │   │   │
       │   │   └─→ Return Payment.Id to client
       │   │
       │   └─→ Response: { paymentId, amount, status }
       │
       ├─── Frontend Redirects User to Payment Gateway
       │    └─→ User enters card details
       │
       ├─── Gateway Processes Payment
       │    └─→ Success or Failure
       │
       ├─── Two Possible Paths:
       │    │
       │    ├─ PAYMENT SUCCESS PATH:
       │    │  │
       │    │  ├─→ Gateway redirects user back
       │    │  │   └─→ With transactionId
       │    │  │
       │    │  ├─→ POST /api/payment/verify?paymentId=xxx&transactionId=yyy
       │    │  │   │
       │    │  │   ├─→ Service: VerifyPaymentAsync()
       │    │  │   │   │
       │    │  │   │   ├─→ Find Payment by Id
       │    │  │   │   │
       │    │  │   │   ├─→ Update Payment.Status = Paid
       │    │  │   │   │
       │    │  │   │   ├─→ Update Payment.CompletedAt = Now
       │    │  │   │   │
       │    │  │   │   ├─→ Update Reservation.Status = Confirmed
       │    │  │   │   │
       │    │  │   │   └─→ Update Reservation.ConfirmedAt = Now
       │    │  │   │
       │    │  │   └─→ Response: Success ✅
       │    │  │
       │    │  └─→ Show "Payment Successful" to user
       │    │
       │    └─ PAYMENT FAILURE PATH:
       │       │
       │       ├─→ User sees error on gateway
       │       │
       │       ├─→ User can retry
       │       │
       │       ├─→ POST /api/payment/initiate (again)
       │       │   └─→ Create NEW Payment record
       │       │
       │       └─→ User completes new payment
       │           └─→ Follows success path
       │
       └─ WEBHOOK PATH (Optional):
          │
          ├─→ Gateway sends: POST /api/payment/webhook
          │   │
          │   ├─→ Service: HandleWebhookAsync()
          │   │   │
          │   │   ├─→ Parse webhook data
          │   │   │
          │   │   ├─→ Find Payment by transactionId
          │   │   │
          │   │   ├─→ If success: Update Payment.Status = Paid
          │   │   │              Update Reservation.Status = Confirmed
          │   │   │
          │   │   ├─→ If failure: Update Payment.Status = Failed
          │   │   │              Store failure reason
          │   │   │
          │   │   └─→ Set WebhookVerified = true
          │   │
          │   └─→ Response: 200 OK
          │
          └─→ System now has confirmed payment status
```

---

## 💳 Payment Status State Machine

```
                    ┌─────────────────┐
                    │   INITIATED     │
                    │  (Service only)  │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │ PENDING (1)     │◄──┐
                    │ Waiting payment  │   │
                    └────┬──────┬─────┘   │
                         │      │        │
            ┌────────────┘      └─────────┼──────┐
            │                              │       │
    ┌───────▼────────┐          ┌────────▼──┐   │
    │ PAID (2) ✅    │          │FAILED (3) │   │
    │ Payment done   │          │Payment err.│   │
    └───────┬────────┘          └──────┬─────┘   │
            │                          │        │
            │                          └────────┘
            │                          (can retry)
            │
            ▼
    ┌──────────────────┐
    │ REFUNDED (4) 🔄  │
    │ Money returned   │
    └──────────────────┘
```

---

## 📊 Data Flow: Request to Response

```
CLIENT REQUEST
    │
    ├─ URL: POST /api/payment/initiate
    ├─ Body: { reservationId, customerId, paymentMethodId }
    │
    └──▶ ┌──────────────────────────────────┐
         │  PaymentController.cs            │
         │  • Get request                   │
         │  • Get user IP address           │
         │  • Validate parameters           │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  IPaymentService                 │
         │  InitiatePaymentAsync()          │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  Validation                      │
         │  • Check reservation exists      │
         │  • Check not already paid        │
         │  • Check reservation valid       │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  Create Payment Entity           │
         │  • Generate new Id               │
         │  • Set Amount from reservation   │
         │  • Set Status = Pending          │
         │  • Set IpAddress for security    │
         │  • Set Timestamps                │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  Repository.Add()                │
         │  • Insert into database          │
         │  • Commit transaction            │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  AutoMapper                      │
         │  • Entity → PaymentResponseDto   │
         │  • Add computed properties       │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  Create Response                 │
         │  • IsSuccess = true              │
         │  • Message = "Payment initiated" │
         │  • Data = PaymentResponseDto     │
         └──────────┬───────────────────────┘
                    │
                    ▼
         ┌──────────────────────────────────┐
         │  PaymentController               │
         │  • Return Ok(result)             │
         │  • HTTP 200 + JSON body          │
         └──────────┬───────────────────────┘
                    │
                    ▼
CLIENT RESPONSE
{
    "isSuccess": true,
    "message": "Payment initiated successfully",
    "data": {
        "id": "550e8400-e29b-41d4-a716-446655440002",
        "reservationId": "550e8400-e29b-41d4-a716-446655440000",
        "customerId": "550e8400-e29b-41d4-a716-446655440001",
        "amount": 150.00,
        "paymentMethodId": 1,
        "paymentStatusId": 1,
        "createdAt": "2024-01-15T10:30:00Z",
        "isSuccessful": false,
        "isPending": true,
        "isFailed": false,
        "isRefunded": false
    },
    "errorCode": null
}
```

---

## 🗄️ Database Relationships

```
┌─────────────────────────────┐
│   CUSTOMERS                  │
├─────────────────────────────┤
│ Id (PK)                     │
│ Name                        │
│ Email                       │
└─────────────────┬───────────┘
                  │ 1:N
                  │
    ┌─────────────▼──────────────┐
    │   RESERVATIONS             │
    ├────────────────────────────┤
    │ Id (PK)                    │
    │ CustomerId (FK)            │
    │ RoomId (FK)                │
    │ CheckInDate                │
    │ CheckOutDate               │
    │ TotalPrice                 │
    │ ReservationStatusId (FK)   │
    └─────────────┬──────────────┘
                  │ 1:N
                  │
    ┌─────────────▼──────────────┐
    │   PAYMENTS                 │
    ├────────────────────────────┤
    │ Id (PK)                    │
    │ ReservationId (FK) ◄─┘     │
    │ CustomerId (FK) ◄──────┘   │
    │ Amount                     │
    │ PaymentMethodId (FK)       │
    │ PaymentStatusId (FK)       │
    │ TransactionId (Unique)     │
    │ GatewayResponse            │
    │ CompletedAt                │
    │ FailedAt                   │
    │ RefundedAt                 │
    │ RowVersion (Concurrency)   │
    └────────────────────────────┘
         │                    │
         │                    └─→ PaymentStatus
         │
         └─→ PaymentMethod
```

---

## 🔍 Error Handling Flow

```
TRY INITIATE PAYMENT
    │
    ├─→ Reservation not found?
    │   └─→ Return ErrorCode.NotFound ❌
    │
    ├─→ Already paid?
    │   └─→ Return ErrorCode.BadRequest ❌
    │
    ├─→ Exception during save?
    │   └─→ Return ErrorCode.ServerError ❌
    │
    └─→ Success?
        └─→ Return ResponseDto.Success ✅
```

---

## 📈 Performance Optimization

```
INDEXES FOR FAST QUERIES
│
├─ IX_Payments_ReservationId
│  └─ Get payments by reservation: 🚀 Fast
│
├─ IX_Payments_CustomerId
│  └─ Get payments by customer: 🚀 Fast
│
├─ IX_Payments_Status
│  └─ Filter by payment status: 🚀 Fast
│
├─ IX_Payments_CreatedAt
│  └─ Date-based queries: 🚀 Fast
│
├─ IX_Payments_TransactionId (Unique)
│  └─ Prevent duplicates: 🚀 Fast lookup
│
├─ IX_Payments_WebhookPending
│  └─ Find pending webhooks: 🚀 Fast
│
└─ IX_Payments_Method_Status (Composite)
   └─ Payment method analytics: 🚀 Fast
```

---

## 🔐 Security Layers

```
USER REQUEST
    │
    ├─ LAYER 1: Authorization
    │  └─ [Authorize] attribute
    │     └─ Check JWT token
    │
    ├─ LAYER 2: Input Validation
    │  └─ Check reservationId not empty
    │     └─ Check customerId valid
    │
    ├─ LAYER 3: Business Logic Validation
    │  └─ Check reservation exists
    │     └─ Check not already paid
    │
    ├─ LAYER 4: Database Constraints
    │  └─ CHECK Amount > 0
    │     └─ CHECK RefundAmount ≤ Amount
    │
    └─ LAYER 5: Data Tracking
       └─ Store IP address
          └─ Store gateway response
             └─ Track timestamps
                └─ Audit trail for fraud detection
```

---

## 🎯 Key Files Location

```
Solution Root
│
├─ Domain/
│  └─ Models/
│     └─ Payment.cs ◄──── Entity Model
│  └─ Enums/
│     ├─ PaymentStatusCode.cs ◄──── Status values
│     └─ PaymentMethodCode.cs ◄──── Method types
│
├─ Application/
│  ├─ Interfaces/
│  │  └─ IPaymentService.cs ◄──── Contract
│  ├─ Services/
│  │  └─ PaymentServices/
│  │     └─ PaymentService.cs ◄──── Implementation
│  ├─ Dtos/
│  │  └─ Payment/
│  │     ├─ PaymentResponseDto.cs
│  │     └─ InitiatePaymentRequestDto.cs
│  └─ MappingProfiles/
│     └─ Payment/
│        └─ PaymentProfile.cs ◄──── AutoMapper
│
├─ Infrastructure/
│  ├─ EntitiesConfigurations/
│  │  └─ PaymentConfiguration.cs ◄──── DB Config
│  └─ Repositories/
│     └─ GenericRepository.cs ◄──── Data access
│
└─ Presentation/
   └─ Controllers/
      └─ PaymentController.cs ◄──── API Endpoints
```

---

Created with ❤️ | Production Ready | Fully Documented
