using Coravel.Invocable;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Jobs
{
    public class CleanupCancelledStockMovementsJob : IInvocable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanupCancelledStockMovementsJob> _logger;

        public CleanupCancelledStockMovementsJob(
            IServiceScopeFactory scopeFactory,
            ILogger<CleanupCancelledStockMovementsJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Invoke()
        {
            using var scope = _scopeFactory.CreateScope();
            var cleanupService = scope.ServiceProvider.GetRequiredService<IStockMovementCleanupService>();

            _logger.LogInformation("CleanupCancelledStockMovementsJob dimulai pada {Time}", DateTime.UtcNow);

            try
            {
                var result = await cleanupService.CleanupCancelledOlderThanDaysAsync(days: 30);
                _logger.LogInformation(
                    "CleanupCancelledStockMovementsJob selesai: Diperiksa={Checked}, Dihapus={Deleted}, Gagal={Failed}",
                    result.CheckedCount, result.DeletedCount, result.FailedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CleanupCancelledStockMovementsJob mengalami exception");
                throw;
            }
        }
    }
}
