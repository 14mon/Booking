Table Users {
Id uuid [pk]
Name string [not null]
Phone string [not null]
Email string [not null, unique]
CountryId uuid [not null, ref: > Countries.Id]
DateOfBirth date
Address string [not null]
ProfileImage string [not null]
RegisterDate datetime [not null]
VerificationToken guid [null]
Status enum('active', 'ban', 'delete') [default: 'active']
FirebaseUserId string [not null]
LoginType string [not null]
IsEmailVerified bool [default: false]
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table Countries {
Id uuid [pk]
Name string [not null]
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table PaymentGateways {
Id uuid [pk]
Platform string [not null]
DisplayName string
CountryId uuid [not null, ref: > Countries.Id]
Image string
IsActive bool [default: true]
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table Packages {
Id uuid [pk]
Name string
Image string
Description string
GatewayId uuid [not null, ref: > PaymentGateways.Id]
Price float
Currency string [not null]
PlanId string [not null, unique]
TermsAndCondition string
IsActive bool [default: true]
Credit int [not null]
ExpiredDuration int [not null] // days valid after purchase
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table Transactions {
Id uuid [pk]
UserId uuid [not null, ref: > Users.Id]
PackageId uuid [not null, ref: > Packages.Id]
ExpiredAt datetime
Amount float
Currency string
Status enum('pending', 'fail', 'success') [default: 'pending']
GatewayOrderId string
GatewayRefCode string
Message string
GatewayRawEventId uuid [ref: - GatewayRawEvents.Id]
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table GatewayRawEvents {
Id uuid [pk]
GatewayOrderId string
EventType string
CallbackResponsePayload json
RequestPayload json
TranResponsePayload json
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table UserCreditHistories {
Id uuid [pk]
UserId uuid
CreditAmount int [not null]
RefundId uuid [ref: - Refunds.Id] // 💡 Optional: for refund traceability
Type enum('purchase','book', 'refund') [not null]
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table Class {
Id uuid [pk]
Name string
CountryId uuid [not null, ref: > Countries.Id]
RequiredCredits int [default: 1]
StartTime datetime [not null]
EndTime datetime [not null]
MaxParticipants int
IsFull bool [default:false]
Status enum('active', 'cancelled', 'completed') [default: 'active']
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table ClassBookings {
Id uuid [pk]
ClassId uuid [not null, ref: > Class.Id]
UserId uuid
UserCreditId uuid [ref: > UserCreditHistories.Id]
BookingStatus enum('waiting', 'booked', 'cancelledByUser', 'cancelByClass') [default: 'waiting']
BookedAt datetime
CancelledAt datetime
IsRefund bool [default: false]
CreatedAt datetime [default: `CURRENT_TIMESTAMP`]
UpdatedAt datetime [default: `CURRENT_TIMESTAMP`]
}

Table Refunds {
Id uuid [pk]
ClassBookingId uuid [not null, ref: - ClassBookings.Id]
RefundType enum('cancelled_before_4hr', 'class_cancelled', 'waitlist_refunded') [not null]
RefundAmount float [not null]
RefundCurrency string [not null]
RefundedAt datetime [default: `CURRENT_TIMESTAMP`]
Notes string
}
