using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class NotificationService : INotificationService
    {

        private readonly INotificationLogRepository _repository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationLogRepository repository,
            ILogger<NotificationService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task NotifySupervisorsMovementCompletedAsync(
            Guid movementId,
            string movementNumber,
            string movementType)
        {
            var log = new NotificationLog
            {
                Title = "Stock movement completed",
                Message = $"Movement {movementNumber} ({movementType}) has been completed.",
                TargetRole = "supervisor",
                RelatedMovementId = movementId,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                // Internal "send": mark sent + write app log.
                // Nanti kalau Modul F diaktifkan, ganti bagian ini ke typed HttpClient.
                log.Status = NotificationStatus.Sent;
                log.SentAt = DateTime.UtcNow;
                await _repository.AddAsync(log);

                _logger.LogInformation(
                    "Notification sent to supervisors for movement {MovementNumber}",
                    movementNumber);
            }
            catch (Exception ex)
            {
                log.Status = NotificationStatus.Failed;
                log.ErrorMessage = ex.Message;
                log.SentAt = DateTime.UtcNow;
                await _repository.AddAsync(log);
                _logger.LogError(ex, "Failed to notify supervisors for {MovementNumber}", movementNumber);
                // Jangan rethrow kalau notifikasi tidak boleh menggagalkan complete movement
            }
        }
    }
}
