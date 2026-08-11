using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Users;

namespace WarehouseWeb.Api.Services;

public interface IUserService
{
    Task<ApiResponse<PaginatedResponse<UserListDto>>> ListUsersAsync(PaginationRequest request);
    Task<ApiResponse<UserDetailDto>> GetUserAsync(Guid id);
    Task<ApiResponse<UserDetailDto>> ActivateUserAsync(Guid id, ActivateUserRequestDto request);
    Task<ApiResponse<UserDetailDto>> DeactivateUserAsync(Guid id);
    Task<ApiResponse<UserDetailDto>> ChangeRoleAsync(Guid id, ChangeRoleRequestDto request);
}
