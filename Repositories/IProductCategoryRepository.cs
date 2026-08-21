using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories;

public interface IProductCategoryRepository
{
    Task<Category?> FindByIdAsync(Guid id);
    Task<List<Category>> FindByIdsAsync(IEnumerable<Guid> ids);
    Task<Category?> FindByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task<List<Category>> ListAsync(string? search, int skip, int take, string sort, string order);
    Task<int> CountAsync(string? search);
}
