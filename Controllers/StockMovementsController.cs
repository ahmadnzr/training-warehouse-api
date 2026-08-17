using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockMovements;
using WarehouseWeb.Api.Services;

namespace WarehouseWeb.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stock-movements")]
    [Authorize]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _service;

        public StockMovementsController(IStockMovementService service)
        {
            _service = service;
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
        [Authorize(Roles = "admin,supervisor,warehouse_operator")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StockMovementDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] StockMovementQueryRequest request)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var result = await _service.ListAsync(request, userId, role);
            return Ok(new ApiResponse<PaginatedResponse<StockMovementDto>>("Movements retrieved successfully", result));
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "admin,supervisor,warehouse_operator")]
        [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetByIdAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return Ok(new ApiResponse<StockMovementDto>("Movement retrieved successfully", result));
        }

        [HttpPost("inbound")]
        [Authorize(Roles = "admin,warehouse_operator")]
        [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateInbound([FromBody] CreateInboundMovementRequestDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _service.CreateInboundDraftAsync(request, userId);
            return Created($"/api/v1/stock-movements/{result.Id}",
                new ApiResponse<StockMovementDto>("Inbound draft created successfully", result));
        }

        [HttpPost("outbound")]
        [Authorize(Roles = "admin,warehouse_operator")]
        [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateOutbound([FromBody] CreateOutboundMovementRequestDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _service.CreateOutboundDraftAsync(request, userId);
            return Created($"/api/v1/stock-movements/{result.Id}",
                new ApiResponse<StockMovementDto>("Outbound draft created successfully", result));
        }

        [HttpPost("transfer")]
        [Authorize(Roles = "admin,warehouse_operator")]
        [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferMovementRequestDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _service.CreateTransferDraftAsync(request, userId);
            return Created($"/api/v1/stock-movements/{result.Id}",
                new ApiResponse<StockMovementDto>("Transfer draft created successfully", result));
        }


        [HttpPost("{id:guid}/complete")]
        [Authorize(Roles = "admin,supervisor")]
        [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Complete(Guid id)
        {
            var result = await _service.CompleteAsync(id);
            return Ok(new ApiResponse<StockMovementDto>("Movement completed successfully", result));
        }

        [HttpPost("{id:guid}/cancel")]
        [Authorize(Roles = "admin,supervisor,warehouse_operator")]
        [ProducesResponseType(typeof(ApiResponse<StockMovementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _service.CancelAsync(id, GetCurrentUserId(), GetCurrentUserRole());
            return Ok(new ApiResponse<StockMovementDto>("Movement cancelled successfully", result));
        }

    }
}
