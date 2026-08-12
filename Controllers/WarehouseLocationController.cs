using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.WarehouseLocations;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class WarehouseLocationsController : ControllerBase
    {
        private readonly IWarehouseLocationService _locationService;

        public WarehouseLocationsController(IWarehouseLocationService locationService)
        {
            _locationService = locationService;
        }

        // GET /api/v1/warehouses/{warehouseId}/locations (Child Routing)
        [HttpGet("warehouses/{warehouseId:guid}/locations")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<WarehouseLocationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(Guid warehouseId, [FromQuery] PaginationRequest request)
        {
            var result = await _locationService.ListAsync(warehouseId, request);
            return Ok(new ApiResponse<PaginatedResponse<WarehouseLocationDto>>("Locations retrieved successfully", result));
        }

        [HttpGet("warehouse-locations/{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<WarehouseLocationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _locationService.GetByIdAsync(id);
            return Ok(new ApiResponse<WarehouseLocationDto>("Location retrieved successfully", result));
        }

        [HttpPost("warehouse-locations")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<WarehouseLocationDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseLocationRequestDto request)
        {
            var result = await _locationService.CreateAsync(request);
            return StatusCode(StatusCodes.Status201Created, new ApiResponse<WarehouseLocationDto>("Location created successfully", result));
        }

        [HttpPut("warehouse-locations/{id:guid}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<WarehouseLocationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseLocationRequestDto request)
        {
            var result = await _locationService.UpdateAsync(id, request);
            return Ok(new ApiResponse<WarehouseLocationDto>("Location updated successfully", result));
        }

        [HttpDelete("warehouse-locations/{id:guid}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _locationService.DeleteAsync(id);
            return Ok(new ApiResponse<bool>("Location deleted successfully", true));
        }
    }
}
