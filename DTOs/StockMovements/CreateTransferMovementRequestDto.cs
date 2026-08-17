namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class CreateTransferMovementRequestDto
    {
        public string? Notes { get; set; }
        public List<CreateTransferMovementItemDto> Items { get; set; } = new();
    }
}
