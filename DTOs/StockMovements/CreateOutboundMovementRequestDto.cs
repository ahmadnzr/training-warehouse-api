namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class CreateOutboundMovementRequestDto
    {
        public string? Notes { get; set; }
        public List<CreateOutboundMovementItemDto> Items { get; set; } = new();
    }
}
