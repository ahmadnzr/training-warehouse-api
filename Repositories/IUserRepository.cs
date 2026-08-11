using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    Task<List<User>> ListAsync(string? search, int skip, int take);
    Task<int> CountAsync(string? search);
    Task UpdateAsync(User user);
}
