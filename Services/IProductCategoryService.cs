using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.ProductCategories;

namespace WarehouseWeb.Api.Services;

public interface IProductCategoryService
{
    Task<PaginatedResponse<ProductCategoryDto>> ListAsync(PaginationRequest request);
    Task<ProductCategoryDto> GetByIdAsync(Guid id);
    Task<ProductCategoryDto> CreateAsync(CreateProductCategoryRequestDto request);
    Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryRequestDto request);
    Task DeleteAsync(Guid id);
}
