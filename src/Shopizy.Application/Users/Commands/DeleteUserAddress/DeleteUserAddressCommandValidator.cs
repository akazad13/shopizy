using FluentValidation;

namespace Shopizy.Application.Users.Commands.DeleteUserAddress;

public class DeleteUserAddressCommandValidator : AbstractValidator<DeleteUserAddressCommand>
{
    public DeleteUserAddressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AddressId).NotEmpty();
    }
}
