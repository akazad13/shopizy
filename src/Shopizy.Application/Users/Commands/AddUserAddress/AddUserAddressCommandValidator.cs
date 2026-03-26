using FluentValidation;

namespace Shopizy.Application.Users.Commands.AddUserAddress;

public class AddUserAddressCommandValidator : AbstractValidator<AddUserAddressCommand>
{
    public AddUserAddressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Street).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).MaximumLength(100);
        RuleFor(x => x.ZipCode).MaximumLength(20);
    }
}
