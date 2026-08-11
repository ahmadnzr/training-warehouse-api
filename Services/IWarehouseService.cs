using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Warehouses;

namespace WarehouseWeb.Api.Services;

public interface IWarehouseService
{
    Task<PaginatedResponse<WarehouseDto>> ListAsync(PaginationRequest request);
    Task<WarehouseDto> GetByIdAsync(Guid id);
    Task<WarehouseDto> CreateAsync(CreateWarehouseRequestDto request);
    Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseRequestDto request);
    Task DeleteAsync(Guid id);

}