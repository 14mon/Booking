namespace booking_system.Models;

public class Class : BaseModel
{
    public string Name { get; set; } = default!;
    public Guid CountryId { get; set; }
    public int RequiredCredit { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxParticipants { get; set; }
    public ClassStatus Status { get; set; }
    public bool IsFull { get; set; } = false;
    public virtual ICollection<ClassBooking>? ClassBookings { get; set; }
    public virtual Country? Country { get; set; }
}