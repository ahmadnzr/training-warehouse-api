using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Suppliers;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SupplierDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] PaginationRequest request)
        {
            var result = await _supplierService.ListAsync(request);
            return Ok(new ApiResponse<PaginatedResponse<SupplierDto>>("Suppliers retrieved successfully", result));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _supplierService.GetByIdAsync(id);
            return Ok(new ApiResponse<SupplierDto>("Supplier retrieved successfully", result));
        }

        [HttpPost]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSupplierRequestDto request)
        {
            var result = await _supplierService.CreateAsync(request);
            return Created($"/api/v1/suppliers/{result.Id}", new ApiResponse<SupplierDto>("Supplier created successfully", result));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierRequestDto request)
        {
            var result = await _supplierService.UpdateAsync(id, request);
            return Ok(new ApiResponse<SupplierDto>("Supplier updated successfully", result));
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _supplierService.DeactivateAsync(id);
            return Ok(new ApiResponse<bool>("Supplier deactivated successfully", true));
        }
    }
}
