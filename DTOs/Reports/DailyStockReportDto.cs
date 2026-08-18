namespace WarehouseWeb.Api.DTOs.Reports
{
    public class DailyStockReportDto
    {
        public Guid Id { get; set; }
        public DateOnly ReportDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Guid? JobExecutionLogId { get; set; }
        public int ItemCount { get; set; }
    }

    public class DailyStockReportDetailDto
    {
        public Guid Id { get; set; }
        public DateOnly ReportDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Guid? JobExecutionLogId { get; set; }
        public List<DailyStockReportItemDto> Items { get; set; } = new();
    }

    public class DailyStockReportItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductSku { get; set; }
        public string? ProductName { get; set; }
        public int TotalQuantity { get; set; }
    }
}
