namespace WarehouseWeb.Api.Models
{
    public class StockMovementItem
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid StockMovementId { get; set; }
        public StockMovement? StockMovement { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public Guid? SourceLocationId { get; set; }
        public WarehouseLocation? SourceLocation { get; set; }

        public Guid? DestinationLocationId { get; set; }
        public WarehouseLocation? DestinationLocation { get; set; }

        public int Quantity { get; set; }
    }

}
