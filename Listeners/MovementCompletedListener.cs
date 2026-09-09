using Coravel.Events.Interfaces;
using WarehouseWeb.Api.Events;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Listeners;

public class MovementCompletedListener : IListener<MovementCompletedEvent>
{
    private readonly INotificationLogRepository _repository;
    private readonly ILogger<MovementCompletedListener> _logger;

    public MovementCompletedListener(
        INotificationLogRepository repository,
        ILogger<MovementCompletedListener> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(MovementCompletedEvent e)
    {
        if (await _repository.ExistsByMovementIdAsync(e.MovementId))
        {
            _logger.LogWarning("Notification already exists for movement {MovementNumber}. Skipping.", e.MovementNumber);
            return;
        }

        var log = new NotificationLog
        {
            Title = "Stock movement completed",
            Message = $"Movement {e.MovementNumber} ({e.MovementType}) has been completed.",
            TargetRole = "supervisor",
            RelatedMovementId = e.MovementId,
            Status = NotificationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            log.Status = NotificationStatus.Sent;
            log.SentAt = DateTime.UtcNow;
            await _repository.AddAsync(log);

            _logger.LogInformation("Notification recorded for movement {MovementNumber}", e.MovementNumber);
        }
        catch (Exception ex)
        {
            log.Status = NotificationStatus.Failed;
            log.ErrorMessage = ex.Message;
            log.SentAt = DateTime.UtcNow;
            await _repository.AddAsync(log);

            _logger.LogError(ex, "Failed to record notification for movement {MovementNumber}", e.MovementNumber);
        }
    }
}
