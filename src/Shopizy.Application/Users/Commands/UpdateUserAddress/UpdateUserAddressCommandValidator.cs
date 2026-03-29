using FluentValidation;

namespace Shopizy.Application.Users.Commands.UpdateUserAddress;

public class UpdateUserAddressCommandValidator : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserAddressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.Street).NotNull().NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).MaximumLength(100);
        RuleFor(x => x.ZipCode).MaximumLength(20);
    }
}
