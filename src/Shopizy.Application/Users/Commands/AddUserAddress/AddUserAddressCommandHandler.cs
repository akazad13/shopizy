using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.AddUserAddress;

public class AddUserAddressCommandHandler(IUserRepository userRepository)
    : ICommandHandler<AddUserAddressCommand, ErrorOr<UserAddress>>
{
    public async Task<ErrorOr<UserAddress>> Handle(
        AddUserAddressCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        var address = user.AddAddress(
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode,
            request.IsDefault
        );

        userRepository.Update(user);

        return address;
    }
}
