### Get Profile
**URL:**
`http://localhost:5098/v1/Users`


### Change Password
**URL:**
`http://localhost:5098/v1/Auth/change-password`

**Payload:**
```json
{
  "email": "eimyaatmon759@gmail.com",
  "oldPassword": "Mon123!@#",
  "newPassword": "Mon321#@!"
}
```

### Reset Password
**URL:**
`http://localhost:5098/v1/Auth/password-reset?Email=eimyatmon759%40gmail.com`

### Register Payload

**URL:**
`http://localhost:5098/v1/Auth/Register`

**Payload:**

```json
{
  "name": "Ei Myat Myat Mon",
  "phone": "09427371700",
  "email": "eimyatmon759@gmail.com",
  "countryId": "9dde76cb-b052-4365-b528-ec2ecc13997b",
  "address": "Yangon",
  "password": "Mon123!@#"
}
```

---

### Login Payload

**URL:**
`http://localhost:5098/v1/Auth/Login`

**Payload:**

```json
{
  "email": "eimyatmon759@gmail.com",
  "password": "Mon123!@#"
}
```

---

### User Booking API Payload

**URL:**
`http://localhost:5098/v1/UserBooking/Class`

**Payload:**

```json
{
  "userId": "5ead963d-9b3a-43e5-aac5-0f19a72e9469",
  "classId": "b6b6a999-4897-439b-a1b1-be770cb5ce6b"
}
```

---

### User Cancel Booking API Payload

**URL:**
`http://localhost:5098/v1/UserBooking/Cancel`

**Payload:**

```json
{
  "userId": "5ead963d-9b3a-43e5-aac5-0f19a72e9469",
  "bookingId": "dd742f1a-27f0-4597-a092-d4b602d7b22e",
  "cancelTime": "2025-06-22T00:00:00.000Z"
}
```

---

### User Cancel Booking Before 4 Hours Payload

**URL:**
`http://localhost:5098/v1/UserBooking/Cancel`

**Payload:**

```json
{
  "userId": "5ead963d-9b3a-43e5-aac5-0f19a72e9469",
  "bookingId": "dd742f1a-27f0-4597-a092-d4b602d7b22e",
  "cancelTime": "2025-06-21T12:57:26.739638Z"
}
```
### Active Package List for each Country
**URL:**
`http://localhost:5098/v1/Packages?$filter=IsActive eq true and PaymentGateway/CountryId eq 9dde76cb-b052-4365-b528-ec2ecc13997b and PaymentGateway/IsActive eq true&$expand=PaymentGateway($expand=Country)`


### User can see the available class schedule list for each country with class info and can book the class
**URL**
`http://localhost:5098/v1/Classes?$filter=Status eq 'active' and IsFull eq false and CountryId eq 9dde76cb-b052-4365-b528-ec2ecc13997b&$expand=Country`



