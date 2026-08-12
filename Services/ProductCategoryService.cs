using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.ProductCategories;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IProductCategoryRepository _categoryRepository;

    public ProductCategoryService(IProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedResponse<ProductCategoryDto>> ListAsync(PaginationRequest request)
    {
        request.Validate();

        var items = await _categoryRepository.ListAsync(
            request.Search,
            request.GetOffset(),
            request.PerPage,
            request.Sort,
            request.Order
        );

        var total = await _categoryRepository.CountAsync(request.Search);

        return new PaginatedResponse<ProductCategoryDto>
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

    public async Task<ProductCategoryDto> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException("Product category not found");
        }

        return MapToDto(category);
    }

    public async Task<ProductCategoryDto> CreateAsync(CreateProductCategoryRequestDto request)
    {
        var exists = await _categoryRepository.ExistsByNameAsync(request.Name);
        if (exists)
        {
            throw new ConflictException("Product category name already exists");
        }

        var category = new Category
        {
            Name = request.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };

        await _categoryRepository.AddAsync(category);

        return MapToDto(category);
    }

    public async Task<ProductCategoryDto> UpdateAsync(
        Guid id,
        UpdateProductCategoryRequestDto request
    )
    {
        var category = await _categoryRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException("Product category not found");
        }

        var nameExists = await _categoryRepository.ExistsByNameAsync(request.Name, id);
        if (nameExists)
        {
            throw new ConflictException("Product category name already exists");
        }

        category.Name = request.Name;

        await _categoryRepository.UpdateAsync(category);

        return MapToDto(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.FindByIdAsync(id);
        if (category == null)
        {
            throw new NotFoundException("Product category not found");
        }

        category.DeletedAt = DateTime.UtcNow;

        await _categoryRepository.UpdateAsync(category);
    }

    private static ProductCategoryDto MapToDto(Category category)
    {
        return new ProductCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        };
    }
}
