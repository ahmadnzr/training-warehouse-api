using FluentValidation;
using WarehouseWeb.Api.DTOs.Suppliers;

namespace WarehouseWeb.Api.Validators.Suppliers
{
    public class CreateSupplierRequestDtoValidator : AbstractValidator<CreateSupplierRequestDto>
    {
        public CreateSupplierRequestDtoValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Phone).MaximumLength(50);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).MaximumLength(255);
            RuleFor(x => x.Address).MaximumLength(500);
        }
    }
}
