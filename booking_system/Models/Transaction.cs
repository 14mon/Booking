
namespace booking_system.Models;

public class Transaction : BaseModel
{
    public Guid UserId { get; set; }
    public DateTime? ExpiredAt { get; set; } //calculate depands on Package's ExpiredDuration
    public string? Platform { get; set; }
    public Guid PackageId { get; set; }
    public float Amount { get; set; }
    public string? Currency { get; set; }
    public string? RequestedPlan { get; set; }
    public string? AppliedPlan { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.pending;
    public string? GateWayOrderId { get; set; }// orderId from external payment GateWay
    public string? GateRefCode { get; set; } //Reference Code from external payment GateWay
    public string? Message { get; set; }
    public Guid GateWayRawEventId { get; set; }
    public virtual User? User { get; set; }
    public virtual Package? Package { get; set; }
    public virtual GatewayRawEvent? GatewayRawEvent { get; set; }

}

