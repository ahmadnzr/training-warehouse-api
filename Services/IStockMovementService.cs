using WarehouseWeb.Api.DTOs.StockMovements;
namespace WarehouseWeb.Api.Services
{
    public interface IStockMovementService
    {
        Task<StockMovementDto> GetByIdAsync(Guid id);
        Task<StockMovementDto> CreateInboundDraftAsync(CreateInboundMovementRequestDto request, Guid userId);
        Task<StockMovementDto> CreateOutboundDraftAsync(CreateOutboundMovementRequestDto request, Guid userId);
        Task<StockMovementDto> CreateTransferDraftAsync(CreateTransferMovementRequestDto request, Guid userId);
    }
}
