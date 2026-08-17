namespace WarehouseWeb.Api.Models
{
    public class StockLevel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public Guid WarehouseLocationId { get; set; }
        public WarehouseLocation? WarehouseLocation { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
