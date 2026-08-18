namespace WarehouseWeb.Api.Models
{
    public class DailyStockReportItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DailyStockReportId { get; set; }
        public DailyStockReport? DailyStockReport { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int TotalQuantity { get; set; }
    }
}
