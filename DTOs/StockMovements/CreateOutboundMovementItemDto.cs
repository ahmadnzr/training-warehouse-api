namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class CreateOutboundMovementItemDto
    {
        public Guid ProductId { get; set; }
        public Guid SourceLocationId { get; set; }
        public int Quantity { get; set; }
    }
}
