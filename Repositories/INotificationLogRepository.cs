using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface INotificationLogRepository
    {
        Task AddAsync(NotificationLog log);
        Task<bool> ExistsByMovementIdAsync(Guid movementId);
        Task<List<NotificationLog>> ListByRecipientAsync(Guid userId, string role, int offset, int limit);
        Task<int> CountByRecipientAsync(Guid userId, string role);
        Task<NotificationLog?> FindByIdAsync(Guid id);
        Task UpdateAsync(NotificationLog log);
        Task DeleteAsync(NotificationLog log);
    }
}
