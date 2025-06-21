namespace booking_system.Models;

public class Country
{
    public Country()
    {
        Users = [];
        PaymentGateways = [];
        Classes = [];
    }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<User>? Users { get; set; }
    public virtual ICollection<PaymentGateway> PaymentGateways { get; set; }
    public virtual ICollection<Class> Classes { get; set; }
}