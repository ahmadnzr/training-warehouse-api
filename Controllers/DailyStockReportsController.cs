using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Jobs;
using WarehouseWeb.Api.DTOs.Reports;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{
    [ApiController]
    [Route("api/v1/reports/daily-stock")]
    [Authorize(Roles = "admin,supervisor")]
    public class DailyStockReportsController : ControllerBase
    {
        private readonly IDailyStockReportService _service;

        public DailyStockReportsController(IDailyStockReportService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<DailyStockReportDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> List([FromQuery] DailyStockReportQueryRequest request)
        {
            var result = await _service.ListAsync(request);
            return Ok(new ApiResponse<PaginatedResponse<DailyStockReportDto>>("Daily stock reports retrieved successfully", result));
        }

        // date format: yyyy-MM-dd
        [HttpGet("{date}")]
        [ProducesResponseType(typeof(ApiResponse<DailyStockReportDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByDate(DateOnly date)
        {
            var result = await _service.GetByDateAsync(date);
            return Ok(new ApiResponse<DailyStockReportDetailDto>("Daily stock report retrieved successfully", result));
        }

        // date format: yyyy-MM-dd (optional body: { "report_date": "2026-08-18" })
        [HttpPost("run")]
        [ProducesResponseType(typeof(ApiResponse<DailyStockReportDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Run([FromQuery] DateOnly? reportDate)
        {
            var result = await _service.GenerateAsync(reportDate, GetUserId());
            var msg = reportDate.HasValue ? $"Daily stock report generated for {reportDate}" : "Daily stock report generated";
            return Ok(new ApiResponse<DailyStockReportDetailDto>(msg, result));
        }

        [HttpGet("job-executions")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<JobExecutionLogDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> JobExecutions([FromQuery] PaginationRequest request)
        {
            var result = await _service.ListJobExecutionsAsync(request);
            return Ok(new ApiResponse<PaginatedResponse<JobExecutionLogDto>>("Job executions retrieved successfully", result));
        }

        private Guid? GetUserId()
        {
            var idClaim = User.FindFirst("id") ?? User.FindFirst("sub");
            return Guid.TryParse(idClaim?.Value, out var id) ? id : null;
        }
    }
}
