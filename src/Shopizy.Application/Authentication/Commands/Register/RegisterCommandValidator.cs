using FluentValidation;

namespace Shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        _ = RuleFor(register => register.FirstName).NotNull().NotEmpty().MaximumLength(50);
        _ = RuleFor(register => register.LastName).NotNull().NotEmpty().MaximumLength(50);
        _ = RuleFor(register => register.Phone).NotNull().NotEmpty().MaximumLength(15);
    }
}
