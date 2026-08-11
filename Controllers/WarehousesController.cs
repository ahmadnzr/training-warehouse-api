using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Warehouses;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<WarehouseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] PaginationRequest request)
    {
        var result = await _warehouseService.ListAsync(request);
        return Ok(new ApiResponse<PaginatedResponse<WarehouseDto>>("Warehouses retrieved successfully", result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _warehouseService.GetByIdAsync(id);
        return Ok(new ApiResponse<WarehouseDto>("Warehouse retrieved successfully", result));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequestDto request)
    {
        var result = await _warehouseService.CreateAsync(request);
        return StatusCode(201, new ApiResponse<WarehouseDto>("Warehouse created successfully", result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ApiResponse<WarehouseDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequestDto request)
    {
        var result = await _warehouseService.UpdateAsync(id, request);
        return Ok(new ApiResponse<WarehouseDto>("Warehouse updated successfully", result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _warehouseService.DeleteAsync(id);
        return Ok(new ApiResponse<bool>("Warehouse deleted successfully", true));
    }

}