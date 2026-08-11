using FluentValidation;
using WarehouseWeb.Api.DTOs.Warehouses;

namespace WarehouseWeb.Api.Validators.Warehouses;
public class UpdateWarehouseRequestDtoValidator : AbstractValidator<UpdateWarehouseRequestDto>
{
    public UpdateWarehouseRequestDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");
    }
}