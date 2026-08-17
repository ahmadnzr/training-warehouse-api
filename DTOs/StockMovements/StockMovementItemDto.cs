namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class StockMovementItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid? SourceLocationId { get; set; }
        public Guid? DestinationLocationId { get; set; }
        public int Quantity { get; set; }
    }
}
