using WarehouseWeb.Api.Models.Enums;

namespace WarehouseWeb.Api.Models;

public class JobExecutionLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string JobName { get; set; } = string.Empty;
    public JobExecutionStatus Status { get; set; } = JobExecutionStatus.Running;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
