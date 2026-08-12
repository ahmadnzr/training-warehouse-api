using FluentValidation;
using WarehouseWeb.Api.DTOs.WarehouseLocations;

namespace WarehouseWeb.Api.Validators.WarehouseLocations
{
    public class UpdateWarehouseLocationRequestDtoValidator : AbstractValidator<UpdateWarehouseLocationRequestDto>
    {
        public UpdateWarehouseLocationRequestDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        }

    }
}
