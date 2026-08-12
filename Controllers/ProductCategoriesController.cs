using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.ProductCategories;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers;

[ApiController]
[Route("api/v1/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoriesController(IProductCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ProductCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] PaginationRequest request)
    {
        var result = await _categoryService.ListAsync(request);
        return Ok(new ApiResponse<PaginatedResponse<ProductCategoryDto>>("Product categories retrieved successfully", result));
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return Ok(new ApiResponse<ProductCategoryDto>("Product category retrieved successfully", result));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProductCategoryRequestDto request)
    {
        var result = await _categoryService.CreateAsync(request);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<ProductCategoryDto>("Product category created successfully", result));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCategoryRequestDto request)
    {
        var result = await _categoryService.UpdateAsync(id, request);
        return Ok(new ApiResponse<ProductCategoryDto>("Product category updated successfully", result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteAsync(id);
        return Ok(new ApiResponse<bool>("Product category deleted successfully", true));
    }
}

