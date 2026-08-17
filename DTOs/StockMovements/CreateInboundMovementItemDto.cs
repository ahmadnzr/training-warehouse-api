namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class CreateInboundMovementItemDto
    {
        public Guid ProductId { get; set; }
        public Guid DestinationLocationId { get; set; }
        public int Quantity { get; set; }
    }
}
