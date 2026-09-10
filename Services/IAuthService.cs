using WarehouseWeb.Api.DTOs.Auth;

namespace WarehouseWeb.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<(AuthResponseDto User, string AccessToken, string RefreshToken)> LoginAsync(LoginRequestDto request);
    Task<(AuthResponseDto User, string AccessToken, string RefreshToken)> RefreshTokenAsync(string oldRefreshToken);
    Task LogoutAsync(string? refreshToken, Guid? userId);
    Task<MeResponseDto> GetMeAsync(Guid userId);
}
