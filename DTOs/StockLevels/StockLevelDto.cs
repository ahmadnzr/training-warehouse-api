namespace WarehouseWeb.Api.DTOs.StockLevels
{
    public class StockLevelDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductSku { get; set; }
        public string? ProductName { get; set; }
        public Guid WarehouseLocationId { get; set; }
        public string? LocationCode { get; set; }
        public Guid? WarehouseId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
