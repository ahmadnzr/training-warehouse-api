using Coravel.Events.Interfaces;

namespace WarehouseWeb.Api.Events;

public class MovementCompletedEvent : IEvent
{
    public Guid MovementId { get; set; }
    public string MovementNumber { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty;
}
