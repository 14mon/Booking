using System.ComponentModel.DataAnnotations.Schema;

namespace booking_system.Models;

public class GatewayRawEvent : BaseModel
{
    public string GateWayOrderId { get; set; } = default!;
    public string EventType { get; set; } = default!; //Atom , MPT, Kpay

    [Column(TypeName = "json")]
    public string? CallbackResponsePayload { get; set; }

    [Column(TypeName = "json")]
    public string? RequestPayload { get; set; }

    [Column(TypeName = "json")]
    public string? TranResponsePayload { get; set; }
    public virtual Transaction? Transaction{ get; set; }
}