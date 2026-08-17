using FluentValidation;
using WarehouseWeb.Api.DTOs.StockMovements;

namespace WarehouseWeb.Api.Validators.StockMovements
{
    public class CreateInboundMovementRequestDtoValidator : AbstractValidator<CreateInboundMovementRequestDto>
    {
        public CreateInboundMovementRequestDtoValidator()
        {
            RuleFor(x => x.SupplierId).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(1000);
            RuleFor(x => x.Items).NotEmpty();
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.DestinationLocationId).NotEmpty();
                item.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
