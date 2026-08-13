
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Products;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{

    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] PaginationRequest request)
        {
            var result = await _productService.ListAsync(request);
            return Ok(new ApiResponse<PaginatedResponse<ProductDto>>("Products retrieved successfully", result));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(new ApiResponse<ProductDto>("Product retrieved successfully", result));
        }

        [HttpPost]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request)
        {
            var result = await _productService.CreateAsync(request);
            return Created($"/api/v1/products/{result.Id}", new ApiResponse<ProductDto>("Product created successfully", result));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequestDto request)
        {
            var result = await _productService.UpdateAsync(id, request);
            return Ok(new ApiResponse<ProductDto>("Product updated successfully", result));
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await _productService.DeactivateAsync(id);
            return Ok(new ApiResponse<bool>("Product deactivated successfully", true));
        }
    }
}
