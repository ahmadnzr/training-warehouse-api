using Coravel.Queuing.Interfaces;
using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockMovements;
using WarehouseWeb.Api.Events;
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
        private readonly IQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockMovementService> _logger;

        private static StockMovementType? ParseType(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Enum.TryParse<StockMovementType>(value, ignoreCase: true, out var type) ? type : null;
        }

        private static StockMovementStatus? ParseStatus(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Enum.TryParse<StockMovementStatus>(value, ignoreCase: true, out var status) ? status : null;
        }

        public StockMovementService(
            IStockMovementRepository movementRepository,
            IProductRepository productRepository,
            IWarehouseLocationRepository locationRepository,
            ISupplierRepository supplierRepository,
            IQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<StockMovementService> logger)
        {
            _movementRepository = movementRepository;
            _productRepository = productRepository;
            _locationRepository = locationRepository;
            _supplierRepository = supplierRepository;
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<StockMovementDto> CreateInboundDraftAsync(CreateInboundMovementRequestDto request, Guid userId)
        {
            var supplier = await _supplierRepository.FindByIdAsync(request.SupplierId);
            if (supplier == null || !supplier.IsActive)
                throw new UnprocessableException("Supplier not found or inactive");

            await EnsureActiveProductsAsync(request.Items.Select(i => i.ProductId));
            await EnsureActiveLocationsAsync(request.Items.Select(i => i.DestinationLocationId));

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
            EnqueueDraftExpirationJob(movement.Id, movement.CreatedAt);
            return MapToDto(movement);
        }

        public async Task<StockMovementDto> CreateOutboundDraftAsync(CreateOutboundMovementRequestDto request, Guid userId)
        {
            await EnsureActiveProductsAsync(request.Items.Select(i => i.ProductId));
            await EnsureActiveLocationsAsync(request.Items.Select(i => i.SourceLocationId));

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
            EnqueueDraftExpirationJob(movement.Id, movement.CreatedAt);
            return MapToDto(movement);
        }

        public async Task<StockMovementDto> CreateTransferDraftAsync(CreateTransferMovementRequestDto request, Guid userId)
        {
            foreach (var item in request.Items)
            {
                if (item.SourceLocationId == item.DestinationLocationId)
                    throw new UnprocessableException("Source and destination location must be different");
            }

            await EnsureActiveProductsAsync(request.Items.Select(i => i.ProductId));
            await EnsureActiveLocationsAsync(request.Items.Select(i => i.SourceLocationId).Concat(request.Items.Select(i => i.DestinationLocationId)));

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
            EnqueueDraftExpirationJob(movement.Id, movement.CreatedAt);
            return MapToDto(movement);
        }

        private async Task EnsureActiveProductsAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            if (!ids.Any()) return;

            var products = await _productRepository.FindByIdsAsync(ids);
            var productMap = products.ToDictionary(p => p.Id);

            var inactiveProducts = ids.Where(id => productMap.TryGetValue(id, out var product) && !product.IsActive).ToList();
            var notFoundProducts = ids.Where(id => !productMap.ContainsKey(id)).ToList();

            if (inactiveProducts.Count != 0 || notFoundProducts.Count != 0)
            {
                var messages = new List<string>();
                if (inactiveProducts.Count != 0)
                    messages.Add($"Some {inactiveProducts.Count} products are inactive: {string.Join(", ", inactiveProducts)}");
                if (notFoundProducts.Count != 0)
                    messages.Add($"Some {notFoundProducts.Count} products are not found: {string.Join(", ", notFoundProducts)}");

                throw new UnprocessableException(string.Join("; ", messages));
            }
        }

        private async Task EnsureActiveLocationsAsync(IEnumerable<Guid> locationIds)
        {
            var ids = locationIds.Distinct().ToList();
            if (!ids.Any()) return;

            var locations = await _locationRepository.FindByIdsAsync(ids);
            var locationMap = locations.ToDictionary(l => l.Id);

            var inactiveLocations = ids.Where(id => locationMap.TryGetValue(id, out var location) && !location.IsActive).ToList();
            var notFoundLocations = ids.Where(id => !locationMap.ContainsKey(id)).ToList();

            if (inactiveLocations.Count != 0 || notFoundLocations.Count != 0)
            {
                var messages = new List<string>();
                if (inactiveLocations.Count != 0)
                    messages.Add($"Some {inactiveLocations.Count} warehouse locations are inactive: {string.Join(", ", inactiveLocations)}");
                if (notFoundLocations.Count != 0)
                    messages.Add($"Some {notFoundLocations.Count} warehouse locations are not found: {string.Join(", ", notFoundLocations)}");

                throw new UnprocessableException(string.Join("; ", messages));
            }
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

        public async Task<PaginatedResponse<StockMovementDto>> ListAsync(StockMovementQueryRequest request, Guid currentUserId, string currentUserRole)
        {
            request.Validate();

            var type = ParseType(request.Type);
            var status = ParseStatus(request.Status);

            if (!string.IsNullOrWhiteSpace(request.Type) && type == null)
                throw new UnprocessableException("Invalid movement type filter");
            if (!string.IsNullOrWhiteSpace(request.Status) && status == null)
                throw new UnprocessableException("Invalid movement status filter");

            // admin & supervisor: lihat semua
            // warehouse_operator: hanya miliknya
            Guid? ownerFilter = currentUserRole == "warehouse_operator" ? currentUserId : null;

            var items = await _movementRepository.ListAsync(
                type,
                status,
                request.ProductId,
                request.DateFrom,
                request.DateTo,
                ownerFilter,
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order);

            var total = await _movementRepository.CountAsync(
                type,
                status,
                request.ProductId,
                request.DateFrom,
                request.DateTo,
                ownerFilter);

            return new PaginatedResponse<StockMovementDto>
            {
                Items = items.Select(MapToDto).ToList(),
                Meta = new PaginationMeta
                {
                    Page = request.Page,
                    PerPage = request.PerPage,
                    Total = total,
                    TotalPage = (int)Math.Ceiling(total / (double)request.PerPage)
                }
            };
        }

        public async Task<StockMovementDto> GetByIdAsync(Guid id, Guid currentUserId, string currentUserRole)
        {

            var movement = await _movementRepository.FindByIdAsync(id);
            if (movement == null) throw new NotFoundException("Movement not found");

            if (currentUserRole == "warehouse_operator" && movement.CreatedByUserId != currentUserId)
                throw new ForbiddenException("You can only view your own movements");

            return MapToDto(movement);
        }

        public async Task<StockMovementDto> CompleteAsync(Guid id)
        {
            var movement = await _movementRepository.FindByIdAsync(id);
            if (movement == null) throw new NotFoundException("Movement not found");

            if (movement.Status != StockMovementStatus.Draft)
                throw new UnprocessableException("Only draft movement can be completed");

            await using var transaction = await _movementRepository.BeginTransactionAsync();
            try
            {
                foreach (var item in movement.Items)
                {
                    if (movement.Type is StockMovementType.Outbound or StockMovementType.Transfer)
                    {
                        var sourceId = item.SourceLocationId
                            ?? throw new UnprocessableException("Source location is required");

                        var sourceLevel = await _movementRepository.GetStockLevelAsync(item.ProductId, sourceId);
                        if (sourceLevel == null || sourceLevel.Quantity < item.Quantity)
                        {
                            throw new UnprocessableException(
                                $"Insufficient stock for product {item.ProductId} at location {sourceId}");
                        }

                        sourceLevel.Quantity -= item.Quantity;
                        sourceLevel.UpdatedAt = DateTime.UtcNow;
                        await _movementRepository.UpdateStockLevelAsync(sourceLevel);
                    }

                    if (movement.Type is StockMovementType.Inbound or StockMovementType.Transfer)
                    {
                        var destId = item.DestinationLocationId
                            ?? throw new UnprocessableException("Destination location is required");

                        var destLevel = await _movementRepository.GetStockLevelAsync(item.ProductId, destId);
                        if (destLevel == null)
                        {
                            destLevel = new StockLevel
                            {
                                ProductId = item.ProductId,
                                WarehouseLocationId = destId,
                                Quantity = item.Quantity,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _movementRepository.AddStockLevelAsync(destLevel);
                        }
                        else
                        {
                            destLevel.Quantity += item.Quantity;
                            destLevel.UpdatedAt = DateTime.UtcNow;
                            await _movementRepository.UpdateStockLevelAsync(destLevel);
                        }
                    }
                }

                movement.Status = StockMovementStatus.Completed;
                movement.CompletedAt = DateTime.UtcNow;
                movement.UpdatedAt = DateTime.UtcNow;

                await _movementRepository.UpdateAsync(movement);

                await transaction.CommitAsync();

                _queue.QueueBroadcast(new MovementCompletedEvent
                {
                    MovementId = movement.Id,
                    MovementNumber = movement.MovementNumber,
                    MovementType = movement.Type.ToString().ToLowerInvariant()
                });

                return MapToDto(movement);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<StockMovementDto> CancelAsync(Guid id, Guid currentUserId, string currentUserRole)
        {
            var movement = await _movementRepository.FindByIdAsync(id);
            if (movement == null) throw new NotFoundException("Movement not found");

            if (movement.Status != StockMovementStatus.Draft)
                throw new UnprocessableException("Only draft movement can be cancelled");

            if (currentUserRole == "warehouse_operator" && movement.CreatedByUserId != currentUserId)
                throw new ForbiddenException("You can only cancel your own movements");

            movement.Status = StockMovementStatus.Cancelled;
            movement.CancelledAt = DateTime.UtcNow;
            movement.UpdatedAt = DateTime.UtcNow;

            await _movementRepository.UpdateAsync(movement);
            return MapToDto(movement);
        }

        public async Task<bool> CancelIfExpiredDraftAsync(Guid id, int expiryHours = 24)
        {
            var movement = await _movementRepository.FindByIdAsync(id);
            if (movement == null || movement.Status != StockMovementStatus.Draft)
            {
                return false;
            }

            var cutoffTime = DateTime.UtcNow.AddHours(-expiryHours);
            if (movement.CreatedAt > cutoffTime)
            {
                return false;
            }

            movement.Status = StockMovementStatus.Cancelled;
            movement.CancelledAt = DateTime.UtcNow;
            movement.UpdatedAt = DateTime.UtcNow;
            movement.Notes = string.IsNullOrWhiteSpace(movement.Notes)
                ? $"Auto-cancelled by system (Expired Draft > {expiryHours} hours)"
                : $"{movement.Notes} | Auto-cancelled by system (Expired Draft > {expiryHours} hours)";

            await _movementRepository.UpdateAsync(movement);
            _logger.LogInformation("Draft movement {MovementNumber} has been automatically cancelled", movement.MovementNumber);
            return true;
        }

        private void EnqueueDraftExpirationJob(Guid movementId, DateTime createdAt, int expiryHours = 24)
        {
            _queue.QueueAsyncTask(() =>
            {
                _ = Task.Run(async () =>
                {
                    var targetTime = createdAt.AddHours(expiryHours);
                    var delay = targetTime - DateTime.UtcNow;
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay);
                    }

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IStockMovementService>();
                        await service.CancelIfExpiredDraftAsync(movementId, expiryHours);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to auto-cancel expired draft {MovementId}", movementId);
                    }
                });

                return Task.CompletedTask;
            });
        }
    }
}
