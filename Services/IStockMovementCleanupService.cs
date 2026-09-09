using WarehouseWeb.Api.DTOs.Jobs;

namespace WarehouseWeb.Api.Services
{
    public interface IStockMovementCleanupService
    {
        Task<CleanupCancelledMovementsResultDto> CleanupCancelledOlderThanDaysAsync(int days = 30);
    }
}
