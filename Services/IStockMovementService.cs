using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockMovements;
namespace WarehouseWeb.Api.Services
{
    public interface IStockMovementService
    {
        Task<PaginatedResponse<StockMovementDto>> ListAsync(
            StockMovementQueryRequest request,
            Guid currentUserId,
            string currentUserRole);

        Task<StockMovementDto> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole);

        Task<StockMovementDto> CreateInboundDraftAsync(CreateInboundMovementRequestDto request, Guid userId);
        Task<StockMovementDto> CreateOutboundDraftAsync(CreateOutboundMovementRequestDto request, Guid userId);
        Task<StockMovementDto> CreateTransferDraftAsync(CreateTransferMovementRequestDto request, Guid userId);

        Task<StockMovementDto> CompleteAsync(Guid id);
        Task<StockMovementDto> CancelAsync(Guid id, Guid currentUserId, string currentUserRole);
    }
}
