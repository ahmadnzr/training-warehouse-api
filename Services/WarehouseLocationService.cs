using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.WarehouseLocations;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{

    public class WarehouseLocationService : IWarehouseLocationService
    {
        private readonly IWarehouseLocationRepository _locationRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseLocationService(
            IWarehouseLocationRepository locationRepository,
            IWarehouseRepository warehouseRepository)
        {
            _locationRepository = locationRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<PaginatedResponse<WarehouseLocationDto>> ListAsync(Guid warehouseId, PaginationRequest request)
        {
            request.Validate();

            var items = await _locationRepository.ListAsync(
                warehouseId,
                request.Search,
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order
            );

            var total = await _locationRepository.CountAsync(warehouseId, request.Search);

            return new PaginatedResponse<WarehouseLocationDto>
            {
                Items = items.Select(MapToDto).ToList(),
                Meta = new PaginationMeta
                {
                    Page = request.Page,
                    PerPage = request.PerPage,
                    Total = total,
                    TotalPage = (int)Math.Ceiling(total / (double)request.PerPage),
                },
            };
        }

        public async Task<WarehouseLocationDto> GetByIdAsync(Guid id)
        {
            var location = await _locationRepository.FindByIdAsync(id);
            if (location == null) throw new NotFoundException("Warehouse location not found");
            return MapToDto(location);
        }

        public async Task<WarehouseLocationDto> CreateAsync(CreateWarehouseLocationRequestDto request)
        {
            var warehouse = await _warehouseRepository.FindByIdAsync(request.WarehouseId);
            if (warehouse == null) throw new NotFoundException("Warehouse not found");

            var exists = await _locationRepository.ExistsByCodeAsync(request.WarehouseId, request.Code);
            if (exists) throw new ConflictException("Warehouse location code already exists in this warehouse");

            var location = new WarehouseLocation
            {
                WarehouseId = request.WarehouseId,
                Code = request.Code,
                Name = request.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Warehouse = warehouse
            };

            await _locationRepository.AddAsync(location);
            return MapToDto(location);
        }

        public async Task<WarehouseLocationDto> UpdateAsync(Guid id, UpdateWarehouseLocationRequestDto request)
        {
            var location = await _locationRepository.FindByIdAsync(id);
            if (location == null) throw new NotFoundException("Warehouse location not found");

            location.Name = request.Name;
            location.IsActive = request.IsActive;
            location.UpdatedAt = DateTime.UtcNow;

            await _locationRepository.UpdateAsync(location);
            return MapToDto(location);
        }

        public async Task DeleteAsync(Guid id)
        {
            var location = await _locationRepository.FindByIdAsync(id);
            if (location == null) throw new NotFoundException("Warehouse location not found");

            location.DeletedAt = DateTime.UtcNow;
            await _locationRepository.UpdateAsync(location);
        }

        private static WarehouseLocationDto MapToDto(WarehouseLocation entity)
        {
            return new WarehouseLocationDto
            {
                Id = entity.Id,
                WarehouseId = entity.WarehouseId,
                WarehouseName = entity.Warehouse?.Name ?? string.Empty,
                Code = entity.Code,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
