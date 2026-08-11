using FluentValidation;
using WarehouseWeb.Api.DTOs.Auth;

namespace WarehouseWeb.Api.Validators.Auth;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Must(HasUpperCase).WithMessage("Password must contain at least one uppercase letter")
            .Must(HasLowerCase).WithMessage("Password must contain at least one lowercase letter")
            .Must(HasDigit).WithMessage("Password must contain at least one digit")
            .Must(HasSpecialChar).WithMessage("Password must contain at least one special character");
    }

    private static bool HasUpperCase(string password) =>
        !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);

    private static bool HasLowerCase(string password) =>
        !string.IsNullOrEmpty(password) && password.Any(char.IsLower);

    private static bool HasDigit(string password) =>
        !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);

    private static bool HasSpecialChar(string password) =>
        !string.IsNullOrEmpty(password) && password.Any(c => !char.IsLetterOrDigit(c));
}
