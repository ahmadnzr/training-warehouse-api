using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.StockLevels;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class StockLevelService : IStockLevelService
    {
        private readonly IStockLevelRepository _stockLevelRepository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseLocationRepository _locationRepository;

        public StockLevelService(
            IStockLevelRepository stockLevelRepository,
            IProductRepository productRepository,
            IWarehouseLocationRepository locationRepository)
        {
            _stockLevelRepository = stockLevelRepository;
            _productRepository = productRepository;
            _locationRepository = locationRepository;
        }

        public async Task<PaginatedResponse<StockLevelDto>> ListAsync(StockLevelQueryRequest request)
        {
            request.Validate();

            var items = await _stockLevelRepository.ListAsync(
                request.ProductId,
                request.WarehouseLocationId,
                request.WarehouseId,
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order);

            var total = await _stockLevelRepository.CountAsync(
                request.ProductId,
                request.WarehouseLocationId,
                request.WarehouseId);

            return new PaginatedResponse<StockLevelDto>
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

        public async Task<IReadOnlyList<StockLevelDto>> ListByProductAsync(Guid productId)
        {
            var product = await _productRepository.FindByIdAsync(productId);
            if (product == null) throw new NotFoundException("Product not found");

            var items = await _stockLevelRepository.ListByProductAsync(productId);
            return items.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<StockLevelDto>> ListByLocationAsync(Guid locationId)
        {
            var location = await _locationRepository.FindByIdAsync(locationId);
            if (location == null) throw new NotFoundException("Warehouse location not found");

            var items = await _stockLevelRepository.ListByLocationAsync(locationId);
            return items.Select(MapToDto).ToList();
        }

        private static StockLevelDto MapToDto(StockLevel entity)
        {
            return new StockLevelDto
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductSku = entity.Product?.Sku,
                ProductName = entity.Product?.Name,
                WarehouseLocationId = entity.WarehouseLocationId,
                LocationCode = entity.WarehouseLocation?.Code,
                WarehouseId = entity.WarehouseLocation?.WarehouseId,
                Quantity = entity.Quantity,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
