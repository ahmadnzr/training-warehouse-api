using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Notifications;

namespace WarehouseWeb.Api.Services
{
    public interface INotificationService
    {
        Task NotifySupervisorsMovementCompletedAsync(Guid movementId, string movementNumber, string movementType);
        Task<PaginatedResponse<NotificationDto>> ListUserNotificationsAsync(Guid userId, string role, PaginationRequest request);
        Task<NotificationDto> CreateAsync(CreateNotificationRequestDto request);
        Task MarkAsReadAsync(Guid id, Guid userId, string role);
        Task DeleteAsync(Guid id, Guid userId, string role);
    }
}
