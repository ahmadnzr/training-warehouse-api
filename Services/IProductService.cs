using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Products;

namespace WarehouseWeb.Api.Services
{
    public interface IProductService
    {

        Task<PaginatedResponse<ProductDto>> ListAsync(PaginationRequest request);
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(CreateProductRequestDto request);
        Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequestDto request);
        Task DeactivateAsync(Guid id);
    }
}
