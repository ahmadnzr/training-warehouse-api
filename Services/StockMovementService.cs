using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockMovements;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Models.Enums;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class StockMovementService : IStockMovementService
    {
        private readonly IStockMovementRepository _movementRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseLocationRepository _locationRepository;
        private readonly ISupplierRepository _supplierRepository;

        public StockMovementService(
            IStockMovementRepository movementRepository,
            IProductRepository productRepository,
            IWarehouseLocationRepository locationRepository,
            ISupplierRepository supplierRepository)
        {
            _movementRepository = movementRepository;
            _productRepository = productRepository;
            _locationRepository = locationRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<StockMovementDto> GetByIdAsync(Guid id)
        {
            var movement = await _movementRepository.FindByIdAsync(id);
            if (movement == null) throw new NotFoundException("Movement not found");
            return MapToDto(movement);
        }

        public async Task<StockMovementDto> CreateInboundDraftAsync(CreateInboundMovementRequestDto request, Guid userId)
        {
            var supplier = await _supplierRepository.FindByIdAsync(request.SupplierId);
            if (supplier == null || !supplier.IsActive)
                throw new UnprocessableException("Supplier not found or inactive");

            foreach (var item in request.Items)
            {
                await EnsureActiveProductAsync(item.ProductId);
                await EnsureActiveLocationAsync(item.DestinationLocationId);
            }

            var movement = new StockMovement
            {
                MovementNumber = GenerateMovementNumber(StockMovementType.Inbound),
                Type = StockMovementType.Inbound,
                Status = StockMovementStatus.Draft,
                SupplierId = request.SupplierId,
                Notes = request.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Items = request.Items.Select(i => new StockMovementItem
                {
                    ProductId = i.ProductId,
                    SourceLocationId = null,
                    DestinationLocationId = i.DestinationLocationId,
                    Quantity = i.Quantity
                }).ToList()
            };

            await _movementRepository.AddAsync(movement);
            return MapToDto(movement);
        }

        public async Task<StockMovementDto> CreateOutboundDraftAsync(CreateOutboundMovementRequestDto request, Guid userId)
        {
            foreach (var item in request.Items)
            {
                await EnsureActiveProductAsync(item.ProductId);
                await EnsureActiveLocationAsync(item.SourceLocationId);
            }

            var movement = new StockMovement
            {
                MovementNumber = GenerateMovementNumber(StockMovementType.Outbound),
                Type = StockMovementType.Outbound,
                Status = StockMovementStatus.Draft,
                SupplierId = null,
                Notes = request.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Items = request.Items.Select(i => new StockMovementItem
                {
                    ProductId = i.ProductId,
                    SourceLocationId = i.SourceLocationId,
                    DestinationLocationId = null,
                    Quantity = i.Quantity
                }).ToList()
            };

            await _movementRepository.AddAsync(movement);
            return MapToDto(movement);
        }

        public async Task<StockMovementDto> CreateTransferDraftAsync(CreateTransferMovementRequestDto request, Guid userId)
        {
            foreach (var item in request.Items)
            {
                if (item.SourceLocationId == item.DestinationLocationId)
                    throw new UnprocessableException("Source and destination location must be different");

                await EnsureActiveProductAsync(item.ProductId);
                await EnsureActiveLocationAsync(item.SourceLocationId);
                await EnsureActiveLocationAsync(item.DestinationLocationId);
            }

            var movement = new StockMovement
            {
                MovementNumber = GenerateMovementNumber(StockMovementType.Transfer),
                Type = StockMovementType.Transfer,
                Status = StockMovementStatus.Draft,
                SupplierId = null,
                Notes = request.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                Items = request.Items.Select(i => new StockMovementItem
                {
                    ProductId = i.ProductId,
                    SourceLocationId = i.SourceLocationId,
                    DestinationLocationId = i.DestinationLocationId,
                    Quantity = i.Quantity
                }).ToList()
            };

            await _movementRepository.AddAsync(movement);
            return MapToDto(movement);
        }

        private async Task EnsureActiveProductAsync(Guid productId)
        {
            var product = await _productRepository.FindByIdAsync(productId);
            if (product == null || !product.IsActive)
                throw new UnprocessableException($"Product {productId} not found or inactive");
        }

        private async Task EnsureActiveLocationAsync(Guid locationId)
        {
            var location = await _locationRepository.FindByIdAsync(locationId);
            if (location == null || !location.IsActive)
                throw new UnprocessableException($"Warehouse location {locationId} not found or inactive");
        }

        private static string GenerateMovementNumber(StockMovementType type)
        {
            var prefix = type switch
            {
                StockMovementType.Inbound => "IN",
                StockMovementType.Outbound => "OUT",
                StockMovementType.Transfer => "TRF",
                _ => "MV"
            };

            return $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpperInvariant()}";
        }

        private static StockMovementDto MapToDto(StockMovement entity)
        {
            return new StockMovementDto
            {
                Id = entity.Id,
                MovementNumber = entity.MovementNumber,
                Type = entity.Type.ToString().ToLowerInvariant(),
                Status = entity.Status.ToString().ToLowerInvariant(),
                SupplierId = entity.SupplierId,
                CreatedByUserId = entity.CreatedByUserId,
                Notes = entity.Notes,
                CompletedAt = entity.CompletedAt,
                CancelledAt = entity.CancelledAt,
                CreatedAt = entity.CreatedAt,
                Items = entity.Items.Select(i => new StockMovementItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    SourceLocationId = i.SourceLocationId,
                    DestinationLocationId = i.DestinationLocationId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
