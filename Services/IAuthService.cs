using WarehouseWeb.Api.DTOs.Auth;

namespace WarehouseWeb.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<MeResponseDto> GetMeAsync(Guid userId);
}
