namespace WarehouseWeb.Api.DTOs.Jobs
{
    public class RunDailyStockReportRequestDto
    {
        // null = pakai tanggal UTC hari ini
        public DateOnly? ReportDate { get; set; }
    }
}
