namespace booking_system.Models;

public class Package : BaseModel
{
    public Package()
    {
        Transactions = [];
    }
    public string Name { get; set; } = default!;
    public string? Image { get; set; }
    public string? Description { get; set; }
    public Guid GatewayId { get; set; }
    public float Price { get; set; }
    public string Currency { get; set; } = default!;
    public string PlanId { get; set; }
    public string? TermsAndCondition { get; set; }
    public bool IsActive { get; set; } = true;
    public int Credit { get; set; }
    public int ExpiredDuration { get; set; }
    public virtual PaymentGateway? PaymentGateway { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
}