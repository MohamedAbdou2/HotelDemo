# 🎯 QUICK REFERENCE CARD - Reservation API Testing

## 🚀 **START HERE - 3 Steps to Test**

### **Step 1: Import to Postman**
1. Open Postman
2. Click **Import**
3. Select `HotelReservation_Postman_Collection.json`
4. Done! ✅

### **Step 2: Update Variables**
1. Click collection name
2. Variables tab
3. Update `baseUrl` to your API URL (default: `https://localhost:7061`)

### **Step 3: Test in Order**
```
1. Login (get token) → 2. Create Reservation → 3. Get Reservation → 4. Initiate Payment
```

---

## 📋 **What I Built for You**

### **New/Modified Files:**
1. ✅ `ReservationController.cs` - New controller with LinkGenerator
2. ✅ `ReservationResponseDto.cs` - Updated with all properties
3. ✅ `ReservationService.cs` - Completed CreateReservation method
4. ✅ `IReservationServices.cs` - Added GetReservationById
5. ✅ `HotelReservation_Postman_Collection.json` - Complete test suite
6. ✅ `TESTING_GUIDE.md` - Full testing guide
7. ✅ `LESSON_LINKGENERATOR.md` - Detailed explanation
8. ✅ `QUICKSTART.md` - This file!

---

## 🎯 **Key Learning Points**

### **✅ YES! Use LinkGenerator - It's Best Practice!**

**Why?**
- ✅ Type-safe URLs
- ✅ Automatic route handling
- ✅ Environment-aware
- ✅ REST compliant

**Example:**
```csharp
// BAD ❌
return Created($"https://localhost/api/Reservation/{id}", data);

// GOOD ✅
var url = _linkGenerator.GetUriByAction(HttpContext, 
    action: nameof(GetReservationById), 
    values: new { id });
return Created(url, data);
```

---

## 📡 **Your Endpoints**

| Method | Endpoint | Purpose | Returns |
|--------|----------|---------|---------|
| POST | `/api/auth/login` | Get JWT token | Token |
| POST | `/api/Reservation` | Create reservation | 201 Created + Location |
| GET | `/api/Reservation/{id}` | Get reservation | Reservation details |
| POST | `/api/Payment/initiate` | Start payment | Payment URL |
| GET | `/api/Payment/history/{id}` | Payment history | Payment list |

---

## 🎨 **Response Pattern (Result Pattern)**

**Success:**
```json
{
  "data": { /* your data */ },
  "isSuccess": true,
  "message": "Success message",
  "errorCode": null
}
```

**Failure:**
```json
{
  "data": null,
  "isSuccess": false,
  "message": "Error description",
  "errorCode": "ValidationError"
}
```

---

## 🔑 **HTTP Status Codes Used**

| Code | Name | When Used |
|------|------|-----------|
| 200 | OK | Successful GET/PUT |
| 201 | Created | Successful POST (resource created) |
| 400 | Bad Request | Validation errors, business logic failures |
| 401 | Unauthorized | Missing/invalid token |
| 404 | Not Found | Resource doesn't exist |

---

## 🧪 **Quick Test - Copy & Paste**

### **1. Create Reservation (Update GUIDs!)**
```json
POST {{baseUrl}}/api/Reservation
Authorization: Bearer {{accessToken}}

{
  "roomId": "YOUR-ROOM-GUID-HERE",
  "customerId": "YOUR-CUSTOMER-GUID-HERE",
  "checkInDate": "2024-12-25T14:00:00Z",
  "checkOutDate": "2024-12-28T11:00:00Z",
  "totalPrice": 450.00
}
```

**Expected Response:**
- Status: `201 Created`
- Header: `Location: .../api/Reservation/{newId}`
- Body: Reservation details + PaymentUrl

---

### **2. Get Reservation**
```
GET {{baseUrl}}/api/Reservation/{id}
Authorization: Bearer {{accessToken}}
```

**Expected Response:**
- Status: `200 OK`
- Body: Full reservation details

---

## 🎓 **What You Learned**

### **LinkGenerator**
- ✅ Generates type-safe URLs
- ✅ Works across environments
- ✅ REST best practice

### **HTTP 201 Created**
- ✅ Indicates resource created
- ✅ Includes Location header
- ✅ Professional API design

### **Result Pattern**
- ✅ Consistent responses
- ✅ Clear success/failure
- ✅ Machine + human readable

### **ViewModel Pattern**
- ✅ Input DTOs (ReservationDto)
- ✅ Output DTOs (ReservationResponseDto)
- ✅ Never expose domain models

---

## 🐛 **Common Issues & Fixes**

### **Issue: 401 Unauthorized**
**Fix:** Run Login request first to get token

### **Issue: 400 Room Not Available**
**Fix:** Check room exists and has `IsAvailable = true`

### **Issue: Invalid GUID format**
**Fix:** Copy actual GUIDs from your database

### **Issue: CheckOut before CheckIn**
**Fix:** Ensure checkOutDate > checkInDate

### **Issue: Reservation expires**
**Fix:** Complete payment within 10 minutes

---

## 📦 **Postman Variables (Auto-saved)**

| Variable | Set By | Used By |
|----------|--------|---------|
| `{{accessToken}}` | Login request | All authenticated endpoints |
| `{{lastReservationId}}` | Create Reservation | Get Reservation, Payment |
| `{{lastPaymentId}}` | Initiate Payment | Payment verification |

---

## 🔄 **Typical Workflow**

```
┌─────────────────┐
│  1. Login       │ → Get {{accessToken}}
└────────┬────────┘
         │
         ▼
┌─────────────────────┐
│  2. Create          │ → Get {{lastReservationId}}
│     Reservation     │ → Get Location URL
│                     │ → Get PaymentUrl
└────────┬────────────┘
         │
         ├──────────────┐
         │              │
         ▼              ▼
┌────────────────┐  ┌──────────────────┐
│  3. Get        │  │  4. Initiate     │
│     Reservation│  │     Payment      │
└────────────────┘  └──────────────────┘
```

---

## 💡 **Pro Tips**

1. **Check Headers Tab** in Postman to see Location header
2. **Use Console** (View → Show Postman Console) to see logs
3. **Test Results Tab** shows test script results
4. **Copy Location URL** from response and paste in browser
5. **Save Requests** to test different scenarios

---

## 📚 **Read More**

- `TESTING_GUIDE.md` - Detailed testing instructions
- `LESSON_LINKGENERATOR.md` - In-depth explanation
- Controller code - Inline comments explain everything

---

## ✅ **Checklist Before Testing**

- [ ] API is running (`dotnet run` or F5 in Visual Studio)
- [ ] Database is updated (`dotnet ef database update`)
- [ ] Postman collection imported
- [ ] baseUrl variable set correctly
- [ ] Have valid room and customer GUIDs ready
- [ ] Read TESTING_GUIDE.md

---

## 🎉 **Success Criteria**

You'll know it works when:
- ✅ Login returns token
- ✅ Create Reservation returns 201
- ✅ Location header has valid URL
- ✅ PaymentUrl is generated
- ✅ Get Reservation returns same data
- ✅ Payment can be initiated

---

## 🆘 **Need Help?**

1. Check `TESTING_GUIDE.md` for detailed steps
2. Check `LESSON_LINKGENERATOR.md` for concepts
3. Check Postman console for errors
4. Check Visual Studio Output window for API logs
5. Verify database has rooms with IsAvailable=true

---

## 🚀 **You're Ready!**

1. Import Postman collection
2. Run tests in order
3. Check responses and headers
4. Read the learning materials
5. Experiment with different scenarios

**Happy Testing!** 🎓

---

**Your Teacher:** GitHub Copilot 🤖  
**Date:** 2024  
**Made for:** Junior Developers Learning REST APIs
