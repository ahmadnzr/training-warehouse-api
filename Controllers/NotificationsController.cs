using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Notifications;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedException("Invalid token");

            return userId;
        }

        private string GetCurrentUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(role))
                throw new UnauthorizedException("Invalid token");
            return role;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<NotificationDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] PaginationRequest request)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var result = await _notificationService.ListUserNotificationsAsync(userId, role, request);
            return Ok(new ApiResponse<PaginatedResponse<NotificationDto>>("Notifications retrieved successfully", result));
        }

        [HttpPost]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequestDto request)
        {
            var result = await _notificationService.CreateAsync(request);
            return Created($"/api/v1/notifications/{result.Id}", new ApiResponse<NotificationDto>("Notification created successfully", result));
        }

        [HttpPatch("{id:guid}/read")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            await _notificationService.MarkAsReadAsync(id, userId, role);
            return Ok(new ApiResponse<bool>("Notification marked as read", true));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            await _notificationService.DeleteAsync(id, userId, role);
            return Ok(new ApiResponse<bool>("Notification deleted successfully", true));
        }
    }
}
