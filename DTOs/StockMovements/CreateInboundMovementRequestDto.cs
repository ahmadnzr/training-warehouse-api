namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class CreateInboundMovementRequestDto
    {
        public Guid SupplierId { get; set; }
        public string? Notes { get; set; }
        public List<CreateInboundMovementItemDto> Items { get; set; } = new();
    }
}
