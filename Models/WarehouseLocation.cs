namespace WarehouseWeb.Api.Models;

public class WarehouseLocation
{
    public Guid Id { get; set; }

    public Guid WarehouseId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Warehouse? Warehouse { get; set; }
}
