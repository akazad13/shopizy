using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Caching;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Commands.UpdateUser;

/// <summary>
/// Handles the <see cref="UpdateUserCommand"/> to update user information.
/// </summary>
public class UpdateUserCommandHandler(IUserRepository userRepository, ICacheHelper cacheHelper)
    : IRequestHandler<UpdateUserCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICacheHelper _cacheHelper = cacheHelper;

    /// <summary>
    /// Handles the update user command.
    /// </summary>
    /// <param name="request">The update user command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success result or an error.</returns>
    public async Task<ErrorOr<Success>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
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

        await _cacheHelper.RemoveAsync($"user-{user.Id.Value}");

        return Result.Success;
    }
}
