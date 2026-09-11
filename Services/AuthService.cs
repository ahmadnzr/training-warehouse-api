using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Auth;
using WarehouseWeb.Api.Helpers;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var exists = await _userRepository.ExistsByEmailAsync(request.Email);
        if (exists)
        {
            throw new ConflictException("Email already registered");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            IsActive = false,
            Role = null,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = RoleHelper.ToSnakeCaseRole(user.Role),
            IsActive = user.IsActive
        };
    }

    public async Task<(AuthResponseDto User, string AccessToken, string RefreshToken)> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("Account is not active. Please contact admin.");
        }

        if (user.Role == null)
        {
            throw new ForbiddenException("No role assigned. Please contact admin.");
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id);

        var userDto = new AuthResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = RoleHelper.ToSnakeCaseRole(user.Role),
            IsActive = user.IsActive
        };

        return (userDto, accessToken, refreshToken.Token);
    }

    public async Task<(AuthResponseDto User, string AccessToken, string RefreshToken)> RefreshTokenAsync(string oldRefreshToken)
    {
        var tokenEntity = await _refreshTokenRepository.FindByTokenAsync(oldRefreshToken);
        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.IsExpired)
        {
            // Token Reuse Detection: Revoke seluruh token aktif user jika token yang sudah dicabut dicoba dipakai kembali
            if (tokenEntity != null && tokenEntity.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllUserTokensAsync(tokenEntity.UserId);
            }
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        var user = tokenEntity.User;
        if (!user.IsActive)
        {
            throw new ForbiddenException("User account is inactive");
        }

        // Token Rotation: Cabut token lama dan terbitkan pasangan token baru
        tokenEntity.IsRevoked = true;
        tokenEntity.RevokedAt = DateTime.UtcNow;

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id);
        tokenEntity.ReplacedByToken = newRefreshToken.Token;

        await _refreshTokenRepository.UpdateAsync(tokenEntity);

        var userDto = new AuthResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = RoleHelper.ToSnakeCaseRole(user.Role),
            IsActive = user.IsActive
        };

        return (userDto, newAccessToken, newRefreshToken.Token);
    }

    public async Task LogoutAsync(string? refreshToken, Guid? userId)
    {
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var token = await _refreshTokenRepository.FindByTokenAsync(refreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(token);
                return;
            }
        }

        if (userId.HasValue)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId.Value);
        }
    }

    public async Task<MeResponseDto> GetMeAsync(Guid userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return new MeResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = RoleHelper.ToSnakeCaseRole(user.Role),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    private string GenerateJwtToken(User user)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured");
        var issuer = _configuration["Jwt:Issuer"] ?? "WarehouseWebApi";
        var audience = _configuration["Jwt:Audience"] ?? "WarehouseWebApi";
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "15");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        if (user.Role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, RoleHelper.ToSnakeCaseRole(user.Role)!));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateAndSaveRefreshTokenAsync(Guid userId)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var tokenString = Convert.ToBase64String(randomBytes);

        var days = int.Parse(_configuration["Jwt:RefreshTokenExpirationInDays"] ?? "7");

        var token = new RefreshToken
        {
            UserId = userId,
            Token = tokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(days),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.AddAsync(token);
        return token;
    }
}

