namespace WarehouseWeb.Api.DTOs.Auth;

public class AuthResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public bool IsActive { get; set; }
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public AuthResponseDto User { get; set; } = new AuthResponseDto();
}
