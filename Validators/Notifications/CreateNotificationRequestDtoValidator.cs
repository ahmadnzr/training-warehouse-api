using FluentValidation;
using WarehouseWeb.Api.DTOs.Notifications;

namespace WarehouseWeb.Api.Validators.Notifications;

public class CreateNotificationRequestDtoValidator : AbstractValidator<CreateNotificationRequestDto>
{
    public CreateNotificationRequestDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters");

        RuleFor(x => x)
            .Must(x => x.RecipientUserId.HasValue || !string.IsNullOrWhiteSpace(x.TargetRole))
            .WithMessage("Either recipient_user_id or target_role must be provided");
    }
}
