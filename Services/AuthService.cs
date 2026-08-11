using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
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

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        var isValidPassword = PasswordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isValidPassword)
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

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            AccessToken = token,
            User = new AuthResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = RoleHelper.ToSnakeCaseRole(user.Role),
                IsActive = user.IsActive
            }
        };
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
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60");

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
}
