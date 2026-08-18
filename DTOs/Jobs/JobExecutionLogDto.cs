namespace WarehouseWeb.Api.DTOs.Jobs
{
    public class JobExecutionLogDto
    {
        public Guid Id { get; set; }
        public string JobName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
