using WarehouseWeb.Api.Models;

namespace WarehouseWeb.Api.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> FindByTokenAsync(string token);
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task RevokeAllUserTokensAsync(Guid userId);
    }
}
