namespace WarehouseWeb.Api.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductCategoryId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal? Weight { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ProductCategory? ProductCategory { get; set; }
    public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public ICollection<StockMovementItem> StockMovementItems { get; set; } =
        new List<StockMovementItem>();
}
