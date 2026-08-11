using FluentValidation;
using WarehouseWeb.Api.DTOs.Users;
using WarehouseWeb.Api.Helpers;

namespace WarehouseWeb.Api.Validators.Users;

public class ChangeRoleRequestDtoValidator : AbstractValidator<ChangeRoleRequestDto>
{
    public ChangeRoleRequestDtoValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(r => RoleHelper.ValidRoles.Contains(r.ToLowerInvariant()))
            .WithMessage("Invalid role. Valid roles: admin, supervisor, warehouse_operator");
    }
}
