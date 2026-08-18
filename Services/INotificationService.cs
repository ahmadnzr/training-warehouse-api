namespace WarehouseWeb.Api.Services
{
    public interface INotificationService
    {
        Task NotifySupervisorsMovementCompletedAsync(Guid movementId, string movementNumber, string movementType);
    }
}
