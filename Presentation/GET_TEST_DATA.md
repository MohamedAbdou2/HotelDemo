# 🔍 How to Get Test Data (Room & Customer GUIDs)

Before you can test the Create Reservation endpoint, you need actual GUIDs from your database.

---

## 🎯 **Quick Solution - SQL Queries**

### **Option 1: Get Available Rooms**
```sql
-- Get rooms that are available for booking
SELECT TOP 5 
    Id, 
    RoomNumber, 
    RoomTypeId,
    PricePerNight,
    IsAvailable
FROM Rooms
WHERE IsAvailable = 1
ORDER BY RoomNumber;
```

**Copy the `Id` from results** - this is your `roomId`

---

### **Option 2: Get Customers**
```sql
-- Get existing customers
SELECT TOP 5 
    Id, 
    FirstName, 
    LastName, 
    Email
FROM Customers
ORDER BY CreatedAt DESC;
```

**Copy the `Id` from results** - this is your `customerId`

---

## 🛠️ **If You Don't Have Test Data**

### **Create a Test Room (SQL)**
```sql
-- First, check what room types exist
SELECT Id, Name, Description FROM RoomTypes;

-- Then create a test room (update RoomTypeId with actual value)
INSERT INTO Rooms (Id, RoomNumber, RoomTypeId, Floor, PricePerNight, MaxOccupancy, IsAvailable, CreatedAt, UpdatedAt)
VALUES (
    NEWID(), 
    '101', 
    'YOUR_ROOM_TYPE_ID_HERE', 
    1, 
    150.00, 
    2, 
    1, 
    GETUTCDATE(), 
    GETUTCDATE()
);

-- Get the newly created room ID
SELECT Id, RoomNumber, PricePerNight FROM Rooms WHERE RoomNumber = '101';
```

---

### **Create a Test Customer (SQL)**
```sql
INSERT INTO Customers (Id, FirstName, LastName, Email, Phone, CreatedAt, UpdatedAt)
VALUES (
    NEWID(),
    'Test',
    'Customer',
    'test.customer@example.com',
    '+1234567890',
    GETUTCDATE(),
    GETUTCDATE()
);

-- Get the newly created customer ID
SELECT Id, FirstName, LastName, Email FROM Customers WHERE Email = 'test.customer@example.com';
```

---

## 🎮 **Alternative: Use Existing Endpoints**

If you have endpoints to get rooms and customers:

### **Get Rooms via API**
```
GET {{baseUrl}}/api/Rooms
Authorization: Bearer {{accessToken}}
```

Response will include room IDs.

### **Get Customers via API**
```
GET {{baseUrl}}/api/Customers
Authorization: Bearer {{accessToken}}
```

Response will include customer IDs.

---

## 📝 **Update Your Postman Request**

Once you have the GUIDs, update the Create Reservation body:

```json
{
  "roomId": "a3b5c7d9-1234-5678-9abc-def012345678",      ← Paste your room GUID
  "customerId": "f1e2d3c4-5678-9abc-def0-123456789abc",  ← Paste your customer GUID
  "checkInDate": "2024-12-25T14:00:00Z",
  "checkOutDate": "2024-12-28T11:00:00Z",
  "totalPrice": 450.00
}
```

---

## 🔥 **Quick Test Data Script**

Run this in your database to create everything needed:

```sql
-- Variables (will be set by the script)
DECLARE @RoomId UNIQUEIDENTIFIER = NEWID();
DECLARE @CustomerId UNIQUEIDENTIFIER = NEWID();
DECLARE @RoomTypeId UNIQUEIDENTIFIER;

-- Get an existing room type (or create one)
SELECT TOP 1 @RoomTypeId = Id FROM RoomTypes;

-- If no room type exists, create one
IF @RoomTypeId IS NULL
BEGIN
    SET @RoomTypeId = NEWID();
    INSERT INTO RoomTypes (Id, Name, Description, BasePrice, CreatedAt, UpdatedAt)
    VALUES (@RoomTypeId, 'Standard', 'Standard room for testing', 100.00, GETUTCDATE(), GETUTCDATE());
END

-- Create test room
INSERT INTO Rooms (Id, RoomNumber, RoomTypeId, Floor, PricePerNight, MaxOccupancy, IsAvailable, CreatedAt, UpdatedAt)
VALUES (@RoomId, '999', @RoomTypeId, 9, 150.00, 2, 1, GETUTCDATE(), GETUTCDATE());

-- Create test customer
INSERT INTO Customers (Id, FirstName, LastName, Email, Phone, CreatedAt, UpdatedAt)
VALUES (@CustomerId, 'Test', 'User', 'test.user@example.com', '+1234567890', GETUTCDATE(), GETUTCDATE());

-- Display the GUIDs you need for Postman
SELECT 
    'Copy these GUIDs into Postman:' as Message,
    @RoomId as RoomId,
    @CustomerId as CustomerId,
    '999' as RoomNumber,
    'test.user@example.com' as CustomerEmail;
```

**Copy the GUIDs from the result!**

---

## ✅ **Verification Checklist**

Before testing, ensure:

- [ ] Room exists in database
- [ ] Room has `IsAvailable = true` (1 in SQL)
- [ ] Customer exists in database
- [ ] You have copied the correct GUID format (with dashes)
- [ ] Dates are in future (checkOut > checkIn)
- [ ] TotalPrice is positive

---

## 🚨 **Common Issues**

### **Issue: "Room not found"**
- Verify room ID exists in database
- Check GUID format (must include dashes)

### **Issue: "Room not available"**
- Check `IsAvailable = 1` in database
- Check no overlapping reservations exist

### **Issue: "Customer not found"**
- Verify customer ID exists in database
- Check GUID format

### **Issue: "Invalid GUID format"**
- GUIDs must be: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- Must include dashes
- Must be 36 characters total

---

## 💡 **Pro Tip: Save Common GUIDs**

In Postman collection variables, add:

```
testRoomId: "your-room-guid"
testCustomerId: "your-customer-guid"
```

Then use in requests:
```json
{
  "roomId": "{{testRoomId}}",
  "customerId": "{{testCustomerId}}",
  ...
}
```

---

## 🎓 **Learning Point**

In a real application, you'd have:
- Admin panel to create rooms/customers
- Seed data in migrations
- Faker library for test data generation
- Automated test fixtures

For now, manual SQL is fine for learning! 📚

---

**Ready to test?** 🚀
1. Run one of the SQL queries above
2. Copy the GUIDs
3. Update Postman request body
4. Test your endpoint!
