
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Jobs;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class StockMovementCleanupService : IStockMovementCleanupService
    {
        private readonly IStockMovementRepository _movementRepository;
        private readonly IJobExecutionLogRepository _jobLogRepository;
        private readonly INotificationLogRepository _notificationLogRepository;
        private readonly ILogger<StockMovementCleanupService> _logger;

        public StockMovementCleanupService(
            IStockMovementRepository movementRepository,
            IJobExecutionLogRepository jobLogRepository,
            INotificationLogRepository notificationLogRepository,
            ILogger<StockMovementCleanupService> logger)
        {
            _movementRepository = movementRepository;
            _jobLogRepository = jobLogRepository;
            _notificationLogRepository = notificationLogRepository;
            _logger = logger;
        }

        public async Task<CleanupCancelledMovementsResultDto> CleanupCancelledOlderThanDaysAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);

            var jobLog = new JobExecutionLog
            {
                JobName = JobNames.CleanupCancelledStockMovements,
                Status = JobExecutionStatus.Running,
                StartedAt = DateTime.UtcNow
            };
            await _jobLogRepository.AddAsync(jobLog);

            int checkedCount = 0;
            int deletedCount = 0;
            int failedCount = 0;

            try
            {
                var candidates = await _movementRepository.GetCancelledMovementsOlderThanAsync(cutoffDate);
                checkedCount = candidates.Count;

                _logger.LogInformation(
                    "Memulai pembersihan stock movement cancelled sebelum {CutoffDate}. Ditemukan: {Count}",
                    cutoffDate, checkedCount);

                // 3. Hapus tiap record dengan isolasi try-catch
                foreach (var movement in candidates)
                {
                    // Guard Clause / Proteksi ganda
                    if (movement.Status != StockMovementStatus.Cancelled)
                    {
                        _logger.LogWarning("Melewati movement {Id} karena status bukan Cancelled", movement.Id);
                        continue;
                    }

                    try
                    {
                        await _movementRepository.HardDeleteAsync(movement);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        _logger.LogError(ex, "Gagal menghapus movement cancelled dengan Id {Id}", movement.Id);
                    }
                }

                jobLog.Status = failedCount > 0 && deletedCount == 0
                    ? JobExecutionStatus.Failed
                    : JobExecutionStatus.Succeeded;
                jobLog.FinishedAt = DateTime.UtcNow;

                if (failedCount > 0)
                {
                    jobLog.ErrorMessage = $"Selesai dengan {failedCount} kegagalan. Diperiksa: {checkedCount}, Dihapus: {deletedCount}";
                }
                await _jobLogRepository.UpdateAsync(jobLog);

                var summaryMessage = $"Cleanup Cancelled Movements selesai. Diperiksa: {checkedCount}, Dihapus: {deletedCount}, Gagal: {failedCount} (Batas: {days} hari).";
                var notification = new NotificationLog
                {
                    Title = "Cleanup Cancelled Stock Movements",
                    Message = summaryMessage,
                    TargetRole = "admin",
                    Status = NotificationStatus.Sent,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = DateTime.UtcNow
                };
                await _notificationLogRepository.AddAsync(notification);

                _logger.LogInformation(summaryMessage);

                return new CleanupCancelledMovementsResultDto
                {
                    CutoffDate = cutoffDate,
                    CheckedCount = checkedCount,
                    DeletedCount = deletedCount,
                    FailedCount = failedCount,
                    ExecutedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                jobLog.Status = JobExecutionStatus.Failed;
                jobLog.FinishedAt = DateTime.UtcNow;
                jobLog.ErrorMessage = ex.Message;
                await _jobLogRepository.UpdateAsync(jobLog);

                _logger.LogError(ex, "Terjadi kesalahan fatal saat menjalankan CleanupCancelledStockMovements");
                throw;
            }
        }
    }
}
