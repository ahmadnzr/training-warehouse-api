using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Auth;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<AuthResponseDto>("User registered successfully", result));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<CookieLoginResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var (user, accessToken, refreshToken) = await _authService.LoginAsync(request);

        AppendAuthCookies(accessToken, refreshToken);

        return Ok(new ApiResponse<CookieLoginResponseDto>("Login successful", new CookieLoginResponseDto { User = user }));
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<CookieLoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            ClearAuthCookies();
            throw new UnauthorizedException("Refresh token is missing");
        }

        try
        {
            var (user, newAccessToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);
            AppendAuthCookies(newAccessToken, newRefreshToken);
            return Ok(new ApiResponse<CookieLoginResponseDto>("Token refreshed successfully", new CookieLoginResponseDto { User = user }));
        }
        catch
        {
            ClearAuthCookies();
            throw;
        }
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

        Guid? userId = null;
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(claim, out var parsedId))
        {
            userId = parsedId;
        }

        await _authService.LogoutAsync(refreshToken, userId);

        ClearAuthCookies();

        return Ok(new ApiResponse<bool>("Logged out successfully", true));
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<MeResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("Invalid token");
        }

        var result = await _authService.GetMeAsync(userId);
        return Ok(new ApiResponse<MeResponseDto>("User retrieved successfully", result));
    }

    private void AppendAuthCookies(string accessToken, string refreshToken)
    {
        var accessExpiryMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "15");
        var refreshExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationInDays"] ?? "7");
        var isSecure = bool.Parse(_configuration["Jwt:Cookie:Secure"] ?? "false") || Request.IsHttps;

        var configuredSameSite = _configuration["Jwt:Cookie:SameSite"];
        var sameSite = Enum.TryParse<SameSiteMode>(configuredSameSite, true, out var parsedSameSite)
            ? parsedSameSite
            : SameSiteMode.Lax;

        var accessCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = sameSite,
            Expires = DateTimeOffset.UtcNow.AddMinutes(accessExpiryMinutes),
            Path = "/"
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = sameSite,
            Expires = DateTimeOffset.UtcNow.AddDays(refreshExpiryDays),
            Path = "/"
        };

        Response.Cookies.Append("access_token", accessToken, accessCookieOptions);
        Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);
    }

    private void ClearAuthCookies()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        };

        Response.Cookies.Delete("access_token", cookieOptions);
        Response.Cookies.Delete("refresh_token", cookieOptions);
    }
}

