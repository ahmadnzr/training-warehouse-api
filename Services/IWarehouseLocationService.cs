
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.WarehouseLocations;

namespace WarehouseWeb.Api.Services
{
    public interface IWarehouseLocationService
    {

        Task<PaginatedResponse<WarehouseLocationDto>> ListAsync(Guid warehouseId, PaginationRequest request);
        Task<WarehouseLocationDto> GetByIdAsync(Guid id);
        Task<WarehouseLocationDto> CreateAsync(CreateWarehouseLocationRequestDto request);
        Task<WarehouseLocationDto> UpdateAsync(Guid id, UpdateWarehouseLocationRequestDto request);
        Task DeleteAsync(Guid id);
    }
}
