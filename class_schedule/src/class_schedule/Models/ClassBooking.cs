namespace booking_system.Models;

public class ClassBooking : BaseModel
{
    public Guid ClassId { get; set; }
    public Guid UserId { get; set; }
    public Guid UserCreditId { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookAt { get; set; }
    public DateTime CancelledAt { get; set; }
    public virtual Class? Class { get; set; }
    public virtual User? User { get; set; }
    public virtual UserCreditHistory? UserCreditHistory { get; set; }
    public virtual Refund? Refund{ get; set; }
}