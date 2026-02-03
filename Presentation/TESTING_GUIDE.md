# 📚 Hotel Reservation API - Testing Guide for Junior Developers

## 🎯 What You'll Learn
- How to use LinkGenerator in ASP.NET Core
- REST API best practices (HTTP status codes, Location headers)
- Result pattern and ViewModel pattern in action
- Testing workflows with Postman

---

## ✅ Why Use LinkGenerator? (Your Question Answered!)

### **YES! LinkGenerator is BEST PRACTICE** 

**Benefits:**
1. ✅ **Type-Safe URLs** - No hardcoded strings, uses action names
2. ✅ **Maintainable** - If routes change, URLs update automatically
3. ✅ **REST Compliant** - Returns proper Location header with 201 Created
4. ✅ **Professional** - Industry standard for resource creation

**Example from your code:**
```csharp
var locationUrl = _linkGenerator.GetUriByAction(
    HttpContext,
    action: nameof(GetReservationById),  // ← Type-safe!
    values: new { id = result.Data!.Id });

return Created(locationUrl, result);  // ← Returns Location header
```

**What the client receives:**
```http
HTTP/1.1 201 Created
Location: https://localhost:7061/api/Reservation/3fa85f64-5717-4562-b3fc-2c963f66afa6
Content-Type: application/json

{
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "paymentUrl": "https://localhost:7061/api/Payment/initiate?reservationId=..."
  },
  "isSuccess": true,
  "message": "Reservation created successfully!"
}
```

---

## 📦 Postman Collection Import Instructions

### **Step 1: Import Collection**
1. Open Postman
2. Click **Import** (top left)
3. Select `HotelReservation_Postman_Collection.json`
4. Click **Import**

### **Step 2: Configure Base URL**
1. Click on the collection name
2. Go to **Variables** tab
3. Update `baseUrl` to match your API:
   - Development: `https://localhost:7061`
   - Production: `https://yourapi.com`

---

## 🧪 Testing Workflow (Follow This Order!)

### **Test 1: Authentication** 🔐
**Endpoint:** `POST /api/auth/login`

**What to do:**
1. Open "Login - Get Token" request
2. Update email/password in body
3. Click **Send**
4. ✅ Check "Test Results" - should say "Token saved successfully!"

**What happens behind the scenes:**
- Postman script automatically extracts token
- Saves to `{{accessToken}}` variable
- All other requests use this token automatically

---

### **Test 2: Create Reservation** 🏨
**Endpoint:** `POST /api/Reservation`

**What to do:**
1. Open "Create Reservation" request
2. Update request body:
   ```json
   {
     "roomId": "YOUR_ROOM_GUID_HERE",
     "customerId": "YOUR_CUSTOMER_GUID_HERE",
     "checkInDate": "2024-12-25T14:00:00Z",
     "checkOutDate": "2024-12-28T11:00:00Z",
     "totalPrice": 450.00
   }
   ```
3. Click **Send**

**Expected Response:**
```json
{
  "data": {
    "id": "newly-created-guid",
    "roomNumber": "101",
    "customerName": "John Doe",
    "status": "Pending",
    "expiresAt": "2024-01-20T10:10:00Z",
    "paymentUrl": "https://localhost:7061/api/Payment/initiate?reservationId=..."
  },
  "isSuccess": true,
  "message": "Reservation created successfully! Please complete the payment within 10 minutes."
}
```

**Check the Headers tab:**
```
Location: https://localhost:7061/api/Reservation/newly-created-guid
```

**What happens:**
1. ✅ Room availability checked
2. ✅ Reservation created with Pending status
3. ✅ Room marked as unavailable
4. ✅ 10-minute timer started (Hangfire background job)
5. ✅ Response includes:
   - 201 Created status
   - Location header with URL to new resource
   - Payment URL for next step
   - Expiration time

---

### **Test 3: Get Reservation by ID** 🔍
**Endpoint:** `GET /api/Reservation/{id}`

**What to do:**
1. Open "Get Reservation by ID" request
2. Notice it uses `{{lastReservationId}}` - automatically set from previous request!
3. Click **Send**

**Alternative:**
- Copy the `Location` header URL from Test 2
- Paste directly into browser or Postman

**Expected Response:**
```json
{
  "data": {
    "id": "guid",
    "roomId": "guid",
    "roomNumber": "101",
    "customerId": "guid",
    "customerName": "John Doe",
    "checkInDate": "2024-12-25T14:00:00Z",
    "checkOutDate": "2024-12-28T11:00:00Z",
    "totalPrice": 450.00,
    "reservationStatusId": 1,
    "status": "Pending",
    "expiresAt": "2024-01-20T10:10:00Z",
    "createdAt": "2024-01-20T10:00:00Z"
  },
  "isSuccess": true,
  "message": "Success"
}
```

**This demonstrates:**
- RESTful GET by ID pattern
- The endpoint that LinkGenerator points to
- Status code 200 OK for successful retrieval

---

### **Test 4: Initiate Payment** 💳
**Endpoint:** `POST /api/Payment/initiate`

**What to do:**
1. Open "Initiate Payment for Reservation" request
2. Body automatically uses `{{lastReservationId}}`
3. Click **Send**
4. **Important:** Do this within 10 minutes of creating reservation!

**Expected Response:**
```json
{
  "data": {
    "paymentId": "guid",
    "reservationId": "guid",
    "amount": 450.00,
    "status": "Pending",
    "checkoutUrl": "https://stripe.com/checkout/..."
  },
  "isSuccess": true,
  "message": "Payment initiated successfully"
}
```

---

## 🧪 Error Scenario Testing

### **Test 5: Room Not Available** ❌
**Purpose:** Ensure system prevents double booking

**How:**
1. Create a reservation for a room
2. Try to create another reservation for same room & dates
3. Should get 400 Bad Request with error message

**Expected Response:**
```json
{
  "data": null,
  "isSuccess": false,
  "message": "The room is already booked for the selected period.",
  "errorCode": "RoomNotAvailable"
}
```

---

### **Test 6: Validation Errors** ❌
**Purpose:** Test FluentValidation is working

**How:**
1. Open "Test: Validation Error" request
2. Contains invalid data (negative price, wrong dates, etc.)
3. Click **Send**

**Expected Response:**
```json
{
  "data": null,
  "isSuccess": false,
  "message": "Validation Failed: \nRoomId: Invalid GUID format; \nCheckOutDate: Must be after CheckInDate; \nTotalPrice: Must be positive",
  "errorCode": "ValidationError"
}
```

---

### **Test 7: Reservation Expires** ⏰
**Purpose:** Test auto-cancellation after 10 minutes

**How:**
1. Create a reservation
2. **Wait 11 minutes** (or adjust Hangfire time in code for testing)
3. Try to get the reservation
4. Status should be "Cancelled"
5. Room should be available again

---

## 🎓 Learning Points for Junior Developers

### **1. HTTP Status Codes** 
Learn when to use each:
- ✅ `200 OK` - Successful GET/PUT
- ✅ `201 Created` - Successful POST (resource created)
- ✅ `400 Bad Request` - Validation errors, business logic failures
- ✅ `404 Not Found` - Resource doesn't exist
- ✅ `401 Unauthorized` - No/invalid token
- ✅ `500 Internal Server Error` - Unhandled exceptions

### **2. REST Principles**
- Resources have unique URLs (`/api/Reservation/{id}`)
- Use proper HTTP verbs (POST=Create, GET=Read)
- Return Location header when creating resources
- Use appropriate status codes

### **3. Result Pattern**
Your `ResponseDto<T>` implements Result pattern:
```csharp
{
  "data": T,              // Actual data
  "isSuccess": bool,      // Quick success check
  "message": string,      // Human-readable message
  "errorCode": ErrorCode? // Machine-readable error
}
```

**Benefits:**
- Consistent response structure
- Easy to handle in frontend
- Clear success/failure indication
- Error codes for i18n/localization

### **4. ViewModel Pattern**
Your DTOs are ViewModels:
- `ReservationDto` - Input from client
- `ReservationResponseDto` - Output to client
- Never expose domain models directly
- Add computed properties (like `PaymentUrl`)

---

## 🔥 Common Mistakes to Avoid

### ❌ **Mistake 1: Hardcoding URLs**
```csharp
// BAD
return Created($"https://localhost:5000/api/Reservation/{id}", result);

// GOOD
var url = _linkGenerator.GetUriByAction(...);
return Created(url, result);
```

### ❌ **Mistake 2: Wrong Status Code**
```csharp
// BAD - Returns 200 OK for creation
return Ok(result);

// GOOD - Returns 201 Created
return Created(locationUrl, result);
```

### ❌ **Mistake 3: Not Testing Edge Cases**
Always test:
- Invalid input
- Resource not found
- Duplicate requests
- Expired resources
- Unauthorized access

### ❌ **Mistake 4: Not Using Postman Variables**
```json
// BAD - Manual ID copying
"reservationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"

// GOOD - Automatic from previous request
"reservationId": "{{lastReservationId}}"
```

---

## 📝 Quick Reference

### **Your API Endpoints:**
```
POST   /api/auth/login                    - Get JWT token
POST   /api/Reservation                   - Create reservation
GET    /api/Reservation/{id}              - Get reservation
POST   /api/Payment/initiate              - Start payment
GET    /api/Payment/history/{reservationId} - Payment history
```

### **Postman Variables:**
- `{{baseUrl}}` - API base URL
- `{{accessToken}}` - JWT token (auto-saved)
- `{{lastReservationId}}` - Last created reservation (auto-saved)
- `{{lastPaymentId}}` - Last payment (auto-saved)

### **Your Response Pattern:**
```json
{
  "data": { /* your data */ },
  "isSuccess": true/false,
  "message": "description",
  "errorCode": "ValidationError" // optional
}
```

---

## 🚀 Next Steps

After mastering this:
1. Add more endpoints (Update, Delete, List reservations)
2. Implement filtering and pagination
3. Add more complex validations
4. Test with real Stripe integration
5. Add logging and monitoring
6. Write unit tests

---

## 💡 Tips for Success

1. **Always test happy path first** (everything works)
2. **Then test edge cases** (errors, validation)
3. **Check headers** (Location, Content-Type)
4. **Use Postman Console** (see auto-saved variables)
5. **Read response messages** (they explain what happened)
6. **Follow the test order** (Auth → Create → Get → Payment)

---

## ❓ Questions to Ask Yourself

After running tests, can you answer:
- ✅ Why does Create return 201 instead of 200?
- ✅ What's in the Location header and why?
- ✅ How does Postman auto-save the reservation ID?
- ✅ What happens if I don't pay within 10 minutes?
- ✅ Why use GUIDs instead of integers for IDs?
- ✅ How does the token authentication work?

If yes - you're ready to build your own APIs! 🎉

---

**Created for:** Junior Developer Training  
**Your Teacher:** GitHub Copilot 🤖  
**Questions?** Check the inline comments in code or Postman descriptions!
