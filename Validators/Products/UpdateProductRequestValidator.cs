using FluentValidation;
using WarehouseWeb.Api.DTOs.Products;

namespace WarehouseWeb.Api.Validators.Products
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequestDto>
    {
        public UpdateProductRequestValidator()
        {

            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Unit).NotEmpty();
            RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        }
    }
}
