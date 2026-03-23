using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.DeleteUserAddress;

public class DeleteUserAddressCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DeleteUserAddressCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(
        DeleteUserAddressCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        var result = user.RemoveAddress(UserAddressId.Create(request.AddressId));
        if (result.IsError) return result.Errors;

        userRepository.Update(user);

        return Result.Deleted;
    }
}
