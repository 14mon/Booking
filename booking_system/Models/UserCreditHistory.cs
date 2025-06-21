namespace booking_system.Models;

public class UserCreditHistory : BaseModel
{
    public UserCreditHistory()
    {
        ClassBookings = [];
    }
    public Guid UserId { get; set; }
    public Guid? RefundId { get; set; } = null;
    public int CreditAmount { get; set; }
    public CreditType Type { get; set; }
    public bool IsExpired { get; set; } = false; //if credit has expired date , should run with schedule which user's credit are expired depands on package
    public virtual User? User { get; set; }
    public virtual ICollection<ClassBooking>? ClassBookings { get; set; }
    public virtual Refund? Refund { get; set; }
}

