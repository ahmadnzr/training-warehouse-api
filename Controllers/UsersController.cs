using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Users;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int per_page = 10,
        [FromQuery] string? search = null)
    {
        var request = new PaginationRequest
        {
            Page = page,
            PerPage = per_page,
            Search = search
        };

        var result = await _userService.ListUsersAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _userService.GetUserAsync(id);
        return Ok(result);
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id, [FromBody] ActivateUserRequestDto request)
    {
        var result = await _userService.ActivateUserAsync(id, request);
        return Ok(result);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _userService.DeactivateUserAsync(id);
        return Ok(result);
    }

    [HttpPatch("{id}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequestDto request)
    {
        var result = await _userService.ChangeRoleAsync(id, request);
        return Ok(result);
    }
}
