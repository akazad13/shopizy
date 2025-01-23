using FluentValidation;

namespace Shopizy.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(register => register.FirstName).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(register => register.LastName).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(register => register.Email).NotNull().NotEmpty().MaximumLength(50).EmailAddress();
        RuleFor(register => register.Password).NotNull().NotEmpty();
    }
}
