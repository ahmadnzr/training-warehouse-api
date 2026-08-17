using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockLevels;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stock-levels")]
    [Authorize(Roles = "admin,supervisor,warehouse_operator")]
    public class StockLevelsController : ControllerBase
    {
        private readonly IStockLevelService _service;

        public StockLevelsController(IStockLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StockLevelDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] StockLevelQueryRequest request)
        {
            var result = await _service.ListAsync(request);
            return Ok(new ApiResponse<PaginatedResponse<StockLevelDto>>("Stock levels retrieved successfully", result));
        }

        [HttpGet("by-product/{productId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StockLevelDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ByProduct(Guid productId)
        {
            var result = await _service.ListByProductAsync(productId);
            return Ok(new ApiResponse<IReadOnlyList<StockLevelDto>>("Stock levels retrieved successfully", result));
        }

        [HttpGet("by-location/{locationId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StockLevelDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ByLocation(Guid locationId)
        {
            var result = await _service.ListByLocationAsync(locationId);
            return Ok(new ApiResponse<IReadOnlyList<StockLevelDto>>("Stock levels retrieved successfully", result));
        }
    }
}
