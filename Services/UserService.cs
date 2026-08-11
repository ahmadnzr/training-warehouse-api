using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Users;
using WarehouseWeb.Api.Helpers;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<PaginatedResponse<UserListDto>>> ListUsersAsync(PaginationRequest request)
    {
        request.Validate();

        var users = await _userRepository.ListAsync(request.Search, request.GetOffset(), request.PerPage);
        var total = await _userRepository.CountAsync(request.Search);

        var items = users.Select(MapToUserListDto).ToList();
        var meta = new PaginationMeta
        {
            Page = request.Page,
            PerPage = request.PerPage,
            Total = total,
            TotalPage = (int)Math.Ceiling(total / (double)request.PerPage)
        };

        return new ApiResponse<PaginatedResponse<UserListDto>>(
            "Users retrieved successfully",
            new PaginatedResponse<UserListDto> { Items = items, Meta = meta });
    }

    public async Task<ApiResponse<UserDetailDto>> GetUserAsync(Guid id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return new ApiResponse<UserDetailDto>("User retrieved successfully", MapToUserDetailDto(user));
    }

    public async Task<ApiResponse<UserDetailDto>> ActivateUserAsync(Guid id, ActivateUserRequestDto request)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (user.IsActive)
        {
            throw new UnprocessableException("User is already active");
        }

        var role = RoleHelper.ParseSnakeCaseRole(request.Role);
        if (role == null)
        {
            throw new ValidationException("role", "Invalid role. Valid roles: admin, supervisor, warehouse_operator");
        }

        user.IsActive = true;
        user.Role = role;
        await _userRepository.UpdateAsync(user);

        return new ApiResponse<UserDetailDto>("User activated successfully", MapToUserDetailDto(user));
    }

    public async Task<ApiResponse<UserDetailDto>> DeactivateUserAsync(Guid id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!user.IsActive)
        {
            throw new UnprocessableException("User is already inactive");
        }

        user.IsActive = false;
        user.Role = null;
        await _userRepository.UpdateAsync(user);

        return new ApiResponse<UserDetailDto>("User deactivated successfully", MapToUserDetailDto(user));
    }

    public async Task<ApiResponse<UserDetailDto>> ChangeRoleAsync(Guid id, ChangeRoleRequestDto request)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (!user.IsActive)
        {
            throw new UnprocessableException("Cannot change role for inactive user");
        }

        var role = RoleHelper.ParseSnakeCaseRole(request.Role);
        if (role == null)
        {
            throw new ValidationException("role", "Invalid role. Valid roles: admin, supervisor, warehouse_operator");
        }

        user.Role = role;
        await _userRepository.UpdateAsync(user);

        return new ApiResponse<UserDetailDto>("Role changed successfully", MapToUserDetailDto(user));
    }

    private static UserListDto MapToUserListDto(User user)
    {
        return new UserListDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = RoleHelper.ToSnakeCaseRole(user.Role),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    private static UserDetailDto MapToUserDetailDto(User user)
    {
        return new UserDetailDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = RoleHelper.ToSnakeCaseRole(user.Role),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
