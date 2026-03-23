using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.UpdateUserAddress;

public class UpdateUserAddressCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateUserAddressCommand, ErrorOr<UserAddress>>
{
    public async Task<ErrorOr<UserAddress>> Handle(
        UpdateUserAddressCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        var result = user.UpdateAddress(
            UserAddressId.Create(request.AddressId),
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode
        );

        if (result.IsError) return result.Errors;

        userRepository.Update(user);

        return result.Value;
    }
}
