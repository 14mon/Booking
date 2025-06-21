namespace booking_system.Models;

public class Refund : BaseModel
{
    public Guid? ClassBookingId { get; set; } //if Refund is false , there is no refund info
    public RedundType Type { get; set; }
    public float RefundAmount { get; set; } //can be credit or price
    public string? RefundCurrency { get; set; } //if refund is credit , will be null

    public DateTime RefundedAt { get; set; }
    public string? Note { get; set; }
    public virtual ClassBooking? ClassBooking { get; set; }

    public virtual UserCreditHistory? UserCreditHistory { get; set; }
}