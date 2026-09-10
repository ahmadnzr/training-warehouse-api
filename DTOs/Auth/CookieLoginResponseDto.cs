namespace WarehouseWeb.Api.DTOs.Auth
{
    public class CookieLoginResponseDto
    {
        public AuthResponseDto User { get; set; } = new();
    }
}
