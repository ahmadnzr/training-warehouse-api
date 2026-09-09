namespace WarehouseWeb.Api.DTOs.Jobs
{
    public class CleanupCancelledMovementsResultDto
    {
        public DateTime CutoffDate { get; set; }
        public int CheckedCount { get; set; }
        public int DeletedCount { get; set; }
        public int FailedCount { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}
