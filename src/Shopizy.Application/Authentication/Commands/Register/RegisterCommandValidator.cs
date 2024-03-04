using FluentValidation;
using Shopizy.Application.Authentication.Commands.Register;

namespace shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(register => register.FirstName).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(register => register.LastName).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(register => register.Phone).NotNull().NotEmpty().MaximumLength(15);
    }
}
