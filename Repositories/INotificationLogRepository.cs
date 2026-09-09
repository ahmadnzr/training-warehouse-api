using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface INotificationLogRepository
    {
        Task AddAsync(NotificationLog log);
        Task<bool> ExistsByMovementIdAsync(Guid movementId);
    }
}
