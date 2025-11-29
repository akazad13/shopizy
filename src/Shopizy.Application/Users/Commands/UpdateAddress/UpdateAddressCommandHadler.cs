using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Commands.UpdateAddress;

/// <summary>
/// Handles the <see cref="UpdateAddressCommand"/> to update user addresses.
/// </summary>
public class UpdateAddressCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateAddressCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;

    /// <summary>
    /// Handles updating a user's address information.
    /// </summary>
    /// <param name="request">The update address command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success result or an error.</returns>
    public async Task<ErrorOr<Success>> Handle(
        UpdateAddressCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.UpdateAddress(
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode
        );

        _userRepository.Update(user);

        if (await _userRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.User.UserNotUpdated;
        }

        return Result.Success;
    }
}
