namespace WarehouseWeb.Api.Models;

public class WarehouseLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WarehouseId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Warehouse? Warehouse { get; set; }
    public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public ICollection<StockMovementItem> SourceMovementItems { get; set; } = new List<StockMovementItem>();
    public ICollection<StockMovementItem> DestinationMovementItems { get; set; } = new List<StockMovementItem>();
}
