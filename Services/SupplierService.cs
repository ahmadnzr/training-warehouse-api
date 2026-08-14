using WarehouseWeb.Api.Common;
using WarehouseWeb.Api.DTOs.Suppliers;
using WarehouseWeb.Api.Models;
using WarehouseWeb.Api.Repositories;

namespace WarehouseWeb.Api.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<PaginatedResponse<SupplierDto>> ListAsync(PaginationRequest request)
        {
            request.Validate();

            var items = await _supplierRepository.ListAsync(
                request.Search,
                request.GetOffset(),
                request.PerPage,
                request.Sort,
                request.Order
            );

            var total = await _supplierRepository.CountAsync(request.Search);

            return new PaginatedResponse<SupplierDto>
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

        public async Task<SupplierDto> GetByIdAsync(Guid id)
        {
            var supplier = await _supplierRepository.FindByIdAsync(id);
            if (supplier == null) throw new NotFoundException("Supplier not found");
            return MapToDto(supplier);
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierRequestDto request)
        {
            if (await _supplierRepository.ExistsByCodeAsync(request.Code))
                throw new ConflictException("Supplier Code already exists");

            var supplier = new Supplier
            {
                Code = request.Code,
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                UserId = request.UserId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _supplierRepository.AddAsync(supplier);
            return MapToDto(supplier);
        }

        public async Task<SupplierDto> UpdateAsync(Guid id, UpdateSupplierRequestDto request)
        {
            var supplier = await _supplierRepository.FindByIdAsync(id);
            if (supplier == null) throw new NotFoundException("Supplier not found");

            supplier.Name = request.Name;
            supplier.Phone = request.Phone;
            supplier.Email = request.Email;
            supplier.Address = request.Address;
            supplier.UserId = request.UserId;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);
            return MapToDto(supplier);
        }

        // Fitur: Supplier yang tidak aktif dapat dinonaktifkan tanpa menghapus data historis.
        public async Task DeactivateAsync(Guid id)
        {
            var supplier = await _supplierRepository.FindByIdAsync(id);
            if (supplier == null) throw new NotFoundException("Supplier not found");

            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _supplierRepository.UpdateAsync(supplier);
        }

        private static SupplierDto MapToDto(Supplier entity)
        {
            return new SupplierDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Phone = entity.Phone,
                Email = entity.Email,
                Address = entity.Address,
                UserId = entity.UserId,
                IsActive = entity.IsActive
            };
        }
    }
}
