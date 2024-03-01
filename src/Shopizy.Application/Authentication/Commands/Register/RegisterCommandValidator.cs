using FluentValidation;
using Shopizy.Application.Authentication.Commands.Register;

namespace shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(registerCommand => registerCommand.FirstName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(registerCommand => registerCommand.LastName).NotNull().NotEmpty().MaximumLength(50);
        RuleFor(registerCommand => registerCommand.Phone).NotNull().NotEmpty().MaximumLength(15);
    }
}
