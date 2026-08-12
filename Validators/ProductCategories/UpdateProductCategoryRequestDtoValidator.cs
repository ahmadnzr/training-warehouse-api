using FluentValidation;
using WarehouseWeb.Api.DTOs.ProductCategories;

namespace WarehouseWeb.Api.Validators.ProductCategories;

public class UpdateProductCategoryRequestDtoValidator
    : AbstractValidator<UpdateProductCategoryRequestDto>
{
    public UpdateProductCategoryRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(150)
            .WithMessage("Category name must not exceed 150 characters");
    }
}
