using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Jobs;
using WarehouseWeb.Api.DTOs.Reports;

namespace WarehouseWeb.Api.Services
{
    public interface IDailyStockReportService
    {
        Task<PaginatedResponse<DailyStockReportDto>> ListAsync(DailyStockReportQueryRequest request);
        Task<DailyStockReportDetailDto> GetByDateAsync(DateOnly date);

        /// <summary>
        /// Generate (or regenerate) report for a date. Always writes job_execution_logs.
        /// </summary>
        Task<DailyStockReportDetailDto> GenerateAsync(DateOnly? reportDate, Guid? triggeredByUserId = null);

        Task<PaginatedResponse<JobExecutionLogDto>> ListJobExecutionsAsync(PaginationRequest request);
    }
}
