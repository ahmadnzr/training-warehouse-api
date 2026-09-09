using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Jobs;
using WarehouseWeb.Api.DTOs.Reports;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers;

[ApiController]
[Route("api/v1/jobs")]
[Authorize(Roles = "admin")]
public class JobsController : ControllerBase
{
    private readonly IDailyStockReportService _reportService;
    private readonly IStockMovementCleanupService _cleanupService;
    private readonly IStockMovementService _stockMovementService;

    public JobsController(
        IDailyStockReportService reportService,
        IStockMovementCleanupService cleanupService,
        IStockMovementService stockMovementService)
    {
        _reportService = reportService;
        _cleanupService = cleanupService;
        _stockMovementService = stockMovementService;
    }

    /// <summary>
    /// Manual trigger — berguna untuk demo & testing tanpa tunggu jam 00:00.
    /// </summary>
    [HttpPost("daily-stock-report/run")]
    [ProducesResponseType(typeof(ApiResponse<DailyStockReportDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RunDailyStockReport([FromBody] RunDailyStockReportRequestDto? request)
    {
        var result = await _reportService.GenerateAsync(request?.ReportDate);
        return Ok(new ApiResponse<DailyStockReportDetailDto>("Daily stock report job finished", result));
    }

    [HttpGet("executions")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobExecutionLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListExecutions([FromQuery] PaginationRequest request)
    {
        var result = await _reportService.ListJobExecutionsAsync(request);
        return Ok(new ApiResponse<PaginatedResponse<JobExecutionLogDto>>("Job executions retrieved successfully", result));
    }

    /// <summary>
    /// Manual trigger pembersihan movement cancelled (> 30 hari).
    /// Memudahkan pengujian dan demo tanpa perlu menunggu jadwal scheduler.
    /// </summary>
    [HttpPost("cleanup-cancelled-movements/run")]
    [ProducesResponseType(typeof(ApiResponse<CleanupCancelledMovementsResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RunCleanupCancelledMovements([FromQuery] int days = 30)
    {
        var result = await _cleanupService.CleanupCancelledOlderThanDaysAsync(days);
        return Ok(new ApiResponse<CleanupCancelledMovementsResultDto>("Cleanup cancelled stock movements job finished", result));
    }

    [HttpPost("cancel-expired-draft/{movementId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelExpiredDraft(Guid movementId, [FromQuery] int expiryHours = 24)
    {
        var cancelled = await _stockMovementService.CancelIfExpiredDraftAsync(movementId, expiryHours);
        return Ok(new ApiResponse<object>(
            cancelled ? "Draft movement successfully auto-cancelled" : "Movement is not eligible for cancellation",
            new { movement_id = movementId, is_cancelled = cancelled }));
    }
}
