using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

/// <summary>
/// Handles the <see cref="UpdatePasswordCommand"/> to update user passwords.
/// </summary>
public class UpdatePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager
) : IRequestHandler<UpdatePasswordCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordManager _passwordManager = passwordManager;

    /// <summary>
    /// Handles updating a user's password with validation and hashing.
    /// </summary>
    /// <param name="request">The update password command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success result or an error.</returns>
    public async Task<ErrorOr<Success>> Handle(
        UpdatePasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.OldPassword == request.NewPassword)
        {
            return CustomErrors.User.PasswordSameAsOld;
        }

        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        if (!_passwordManager.Verify(request.OldPassword, user.Password ?? ""))
        {
            return CustomErrors.User.PasswordNotCorrect;
        }

        user.UpdatePassword(_passwordManager.CreateHashString(request.NewPassword));

        _userRepository.Update(user);

        return Result.Success;
    }
}
