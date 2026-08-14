using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Suppliers;

namespace WarehouseWeb.Api.Services
{
    public interface ISupplierService
    {
        Task<PaginatedResponse<SupplierDto>> ListAsync(PaginationRequest request);
        Task<SupplierDto> GetByIdAsync(Guid id);
        Task<SupplierDto> CreateAsync(CreateSupplierRequestDto request);
        Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierRequestDto request);
        Task DeactivateAsync(Guid id);

    }
}
