using FluentValidation;
using WarehouseWeb.Api.DTOs.Products;

namespace WarehouseWeb.Api.Validators.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequestDto>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Unit).NotEmpty();
            RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        }
    }
}
