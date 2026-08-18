using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
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
        public async Task<IActionResult> List([FromQuery] DailyStockReportQueryRequest request)
        {
            var result = await _service.ListAsync(request);
            return Ok(new ApiResponse<PaginatedResponse<DailyStockReportDto>>("Daily stock reports retrieved successfully", result));
        }

        // date format: yyyy-MM-dd
        [HttpGet("{date}")]
        [ProducesResponseType(typeof(ApiResponse<DailyStockReportDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByDate(DateOnly date)
        {
            var result = await _service.GetByDateAsync(date);
            return Ok(new ApiResponse<DailyStockReportDetailDto>("Daily stock report retrieved successfully", result));
        }
    }
}
