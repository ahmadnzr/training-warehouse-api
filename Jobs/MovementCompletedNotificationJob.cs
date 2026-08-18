using Coravel.Invocable;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Jobs
{
    public class MovementCompletedNotificationJob
    {

        private readonly IServiceScopeFactory _scopeFactory;
        public Guid MovementId { get; set; }
        public string MovementNumber { get; set; } = string.Empty;
        public string MovementType { get; set; } = string.Empty;

        public MovementCompletedNotificationJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke()
        {
            using var scope = _scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notificationService.NotifySupervisorsMovementCompletedAsync(
                MovementId,
                MovementNumber,
                MovementType);
        }
    }
}
