using FluentValidation;
using WarehouseWeb.Api.DTOs.StockMovements;

namespace WarehouseWeb.Api.Validators.StockMovements
{
    public class CreateOutboundMovementRequestDtoValidator : AbstractValidator<CreateOutboundMovementRequestDto>
    {
        public CreateOutboundMovementRequestDtoValidator()
        {
            RuleFor(x => x.Items).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(1000);
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.SourceLocationId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
