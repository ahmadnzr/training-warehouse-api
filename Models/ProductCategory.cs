namespace WarehouseWeb.Api.Models;

public class ProductCategory
{
    public Guid ProductId { get; set; } = Guid.NewGuid();
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
    public Category? Category { get; set; }
}
