using FluentValidation;
using WarehouseWeb.Api.DTOs.StockMovements;

namespace WarehouseWeb.Api.Validators.StockMovements
{
    public class CreateTransferMovementRequestDtoValidator : AbstractValidator<CreateTransferMovementRequestDto>
    {
        public CreateTransferMovementRequestDtoValidator()
        {
            RuleFor(x => x.Items).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(1000);
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.SourceLocationId).NotEmpty();
                item.RuleFor(i => i.DestinationLocationId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i)
                    .Must(i => i.SourceLocationId != i.DestinationLocationId)
                    .WithMessage("Source and destination location must be different");
            });
        }

    }
}


