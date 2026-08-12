using FluentValidation;
using WarehouseWeb.Api.DTOs.WarehouseLocations;

namespace WarehouseWeb.Api.Validators.WarehouseLocations
{
    public class CreateWarehouseLocationRequestDtoValidator : AbstractValidator<CreateWarehouseLocationRequestDto>
    {
        public CreateWarehouseLocationRequestDtoValidator()
        {
            RuleFor(x => x.WarehouseId).NotEmpty().WithMessage("Warehouse ID is required");
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }
    }
}
