using FluentValidation;

namespace Shopizy.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).MaximumLength(20).When(x => x.PhoneNumber is not null);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(100).When(x => x.Street is not null);
        RuleFor(x => x.City).NotEmpty().MaximumLength(50).When(x => x.City is not null);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50).When(x => x.State is not null);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50).When(x => x.Country is not null);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(20).When(x => x.ZipCode is not null);
    }
}
