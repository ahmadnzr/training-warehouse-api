namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class CreateTransferMovementItemDto
    {
        public Guid ProductId { get; set; }
        public Guid SourceLocationId { get; set; }
        public Guid DestinationLocationId { get; set; }
        public int Quantity { get; set; }
    }
}
