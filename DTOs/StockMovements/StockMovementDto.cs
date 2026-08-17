namespace WarehouseWeb.Api.DTOs.StockMovements
{
    public class StockMovementDto
    {
        public Guid Id { get; set; }
        public string MovementNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<StockMovementItemDto> Items { get; set; } = new();
    }
}
