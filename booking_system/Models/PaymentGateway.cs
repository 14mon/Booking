namespace booking_system.Models;

public class PaymentGateway : BaseModel
{
    public PaymentGateway()
    {
        Packages = [];
    }
    public string Platform { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public Guid CountryId { get; set; }
    public string? Image { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual Country? Country { get; set; }
    public virtual ICollection<Package>? Packages { get; set; }

}