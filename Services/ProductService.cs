using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Products;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;

        public ProductService(
            IProductRepository productRepository,
            IProductCategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = categoryRepository;
        }

        public async Task<PaginatedResponse<ProductDto>> ListAsync(PaginationRequest request)
        {
            request.Validate();

            var items = await _productRepository.ListAsync(
                request.Search,
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order
            );

            var total = await _productRepository.CountAsync(request.Search);

            return new PaginatedResponse<ProductDto>
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

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null) throw new NotFoundException("Product not found");
            return MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequestDto request)
        {
            var exists = await _productRepository.ExistsBySkuAsync(request.Sku);
            if (exists) throw new ConflictException("SKU already exists");

            var product = new Product
            {
                Sku = request.Sku,
                Name = request.Name,
                Unit = request.Unit,
                Weight = request.Weight,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                foreach (var categoryId in request.CategoryIds.Distinct())
                {
                    var category = await _productCategoryRepository.FindByIdAsync(categoryId);
                    if (category != null)
                    {
                        product.ProductCategories.Add(new ProductCategory
                        {
                            CategoryId = category.Id
                        });
                    }
                }
            }

            await _productRepository.AddAsync(product);
            return MapToDto(product);
        }

        public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequestDto request)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null) throw new NotFoundException("Product not found");

            product.Name = request.Name;
            product.Unit = request.Unit;
            product.Weight = request.Weight;
            product.UpdatedAt = DateTime.UtcNow;

            if (request.CategoryIds != null)
            {
                product.ProductCategories.Clear();

                foreach (var categoryId in request.CategoryIds.Distinct())
                {
                    var category = await _productCategoryRepository.FindByIdAsync(categoryId);
                    if (category != null)
                    {
                        product.ProductCategories.Add(new ProductCategory
                        {
                            CategoryId = category.Id
                        });
                    }
                }
            }

            await _productRepository.UpdateAsync(product);
            return MapToDto(product);
        }

        public async Task DeactivateAsync(Guid id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null) throw new NotFoundException("Product not found");

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
        }

        private static ProductDto MapToDto(Product entity)
        {
            return new ProductDto
            {
                Id = entity.Id,
                Sku = entity.Sku,
                Name = entity.Name,
                Unit = entity.Unit,
                Weight = entity.Weight ?? 0m,
                IsActive = entity.IsActive,
                CategoryNames = entity.ProductCategories?
                    .Where(pc => pc.Category != null)
                    .Select(pc => pc.Category!.Name)
                    .ToList() ?? new List<string>()
            };
        }
    }
}
