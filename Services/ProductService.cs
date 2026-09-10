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
        private readonly ICacheService _cacheService;

        public ProductService(
            IProductRepository productRepository,
            IProductCategoryRepository categoryRepository,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _productCategoryRepository = categoryRepository;
            _cacheService = cacheService;
        }

        public async Task<PaginatedResponse<ProductDto>> ListAsync(PaginationRequest request)
        {
            request.Validate();

            var cacheKey = $"products:list:search={request.Search?.Trim().ToLowerInvariant()}:page={request.Page}:per_page={request.PerPage}:sort={request.Sort?.ToLowerInvariant()}:order={request.Order?.ToLowerInvariant()}";

            return await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var items = await _productRepository.ListAsync(
                        request.Search,
                        request.GetOffset(),
                        request.PerPage,
                        request.Sort ?? "created_at",
                        request.Order ?? "desc"
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
                },
                TimeSpan.FromMinutes(5)
            );
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var cacheKey = $"products:detail:{id}";

            return await _cacheService.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var product = await _productRepository.FindByIdAsync(id);
                    if (product == null) throw new NotFoundException("Product not found");
                    return MapToDto(product);
                },
                TimeSpan.FromMinutes(10)
            );
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
                var categoryIds = request.CategoryIds.Distinct().ToList();
                var categories = await _productCategoryRepository.FindByIdsAsync(categoryIds);

                foreach (var category in categories)
                {
                    product.ProductCategories.Add(new ProductCategory
                    {
                        CategoryId = category.Id
                    });
                }
            }

            await _productRepository.AddAsync(product);
            await _cacheService.RemoveByPrefixAsync("products:list:");
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

                if (request.CategoryIds.Any())
                {
                    var categoryIds = request.CategoryIds.Distinct().ToList();
                    var categories = await _productCategoryRepository.FindByIdsAsync(categoryIds);

                    foreach (var category in categories)
                    {
                        product.ProductCategories.Add(new ProductCategory
                        {
                            CategoryId = category.Id
                        });
                    }
                }
            }

            await _productRepository.UpdateAsync(product);
            await _cacheService.RemoveAsync($"products:detail:{id}");
            await _cacheService.RemoveByPrefixAsync("products:list:");
            return MapToDto(product);
        }

        public async Task DeactivateAsync(Guid id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null) throw new NotFoundException("Product not found");

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            await _cacheService.RemoveAsync($"products:detail:{id}");
            await _cacheService.RemoveByPrefixAsync("products:list:");
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
