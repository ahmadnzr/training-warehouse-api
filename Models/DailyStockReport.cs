namespace WarehouseWeb.Api.Models
{
    public class DailyStockReport
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        // Date only — di EF pakai DateOnly (NET 6+) atau DateTime dengan .HasColumnType("date")
        public DateOnly ReportDate { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public Guid? JobExecutionLogId { get; set; }
        public JobExecutionLog? JobExecutionLog { get; set; }

        public ICollection<DailyStockReportItem> Items { get; set; } = new List<DailyStockReportItem>();
    }
}
