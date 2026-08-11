using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Warehouses;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    private static WarehouseDto MapToDto(Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Code = warehouse.Code,
            Name = warehouse.Name,
            Address = warehouse.Address,
            City = warehouse.City,
            IsActive = warehouse.IsActive,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt
        };
    }


    public async Task<WarehouseDto> CreateAsync(CreateWarehouseRequestDto request)
    {
        var exists = await _warehouseRepository.ExistsByCodeAsync(request.Code);
        if (exists)
        {
            throw new ConflictException("Warehouse code already exists");
        }

        var warehouse = new Warehouse
        {
            Code = request.Code,
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _warehouseRepository.AddAsync(warehouse);

        return MapToDto(warehouse);

    }

    public async Task<WarehouseDto> GetByIdAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.FindByIdAsync(id);
        if (warehouse == null)
        {
            throw new NotFoundException("Warehouse not found");
        }

        return MapToDto(warehouse);

    }

    public async Task<PaginatedResponse<WarehouseDto>> ListAsync(PaginationRequest request)
    {
        request.Validate();

        var items = await _warehouseRepository.ListAsync(
            request.Search,
            request.GetOffset(),
            request.PerPage,
            request.Sort,
            request.Order);

        var total = await _warehouseRepository.CountAsync(request.Search);

        return new PaginatedResponse<WarehouseDto>
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

    public async Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseRequestDto request)
    {
        var warehouse = await _warehouseRepository.FindByIdAsync(id);
        if (warehouse == null)
        {
            throw new NotFoundException("Warehouse not found");
        }

        var codeExists = await _warehouseRepository.ExistsByCodeAsync(request.Code, id);
        if (codeExists)
        {
            throw new ConflictException("Warehouse code already exists");
        }

        warehouse.Code = request.Code;
        warehouse.Name = request.Name;
        warehouse.Address = request.Address;
        warehouse.City = request.City;

        await _warehouseRepository.UpdateAsync(warehouse);

        return MapToDto(warehouse);

    }
    public async Task DeleteAsync(Guid id)
    {
        var warehouse = await _warehouseRepository.FindByIdAsync(id);
        if (warehouse == null)
        {
            throw new NotFoundException("Warehouse not found");
        }

        warehouse.DeletedAt = DateTime.UtcNow;

        await _warehouseRepository.UpdateAsync(warehouse);
    }

}