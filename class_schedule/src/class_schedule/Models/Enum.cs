public enum UserStatus
{
    active,
    ban,
    delete
}
public enum TransactionStatus
{
    pending,
    success,
    fail
}
public enum CreditType
{
    purchase,
    book,
    refund
}
public enum ClassStatus
{
    active,
    cancelled,
    completed
}
public enum BookingStatus
{
    waiting,
    booked,
    cancelledbyUser,
    cancelledbyClass
}
public enum RedundType
{
    cancelled_before_4hr,
    class_cancelled,
    waitlist_refunded
}