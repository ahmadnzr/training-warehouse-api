using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories;

public interface IWarehouseRepository
{
    Task<Warehouse?> FindByIdAsync(Guid id);
    Task<Warehouse?> FindByCodeAsync(string code);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    Task AddAsync(Warehouse warehouse);
    Task UpdateAsync(Warehouse warehouse);
    Task<List<Warehouse>> ListAsync(string? search, int skip, int take, string sort, string order);
    Task<int> CountAsync(string? search);
}