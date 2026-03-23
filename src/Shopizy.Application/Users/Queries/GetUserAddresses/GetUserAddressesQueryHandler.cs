using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Queries.GetUserAddresses;

public class GetUserAddressesQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserAddressesQuery, ErrorOr<List<UserAddress>>>
{
    public async Task<ErrorOr<List<UserAddress>>> Handle(
        GetUserAddressesQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        return user.Addresses.ToList();
    }
}
