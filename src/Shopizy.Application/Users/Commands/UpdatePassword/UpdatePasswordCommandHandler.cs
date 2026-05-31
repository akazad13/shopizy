using ErrorOr;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

/// <summary>
/// Handles the <see cref="UpdatePasswordCommand"/> to update user passwords.
/// </summary>
/// <param name="userRepository"></param>
/// <param name="passwordManager"></param>
public class UpdatePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager
) : ICommandHandler<UpdatePasswordCommand, ErrorOr<Success>>
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
        CancellationToken cancellationToken = default
    )
    {
        if (request.OldPassword == request.NewPassword)
        {
            return (Error)CustomErrors.User.PasswordSameAsOld;
        }

        var user = await _userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return (Error)CustomErrors.User.UserNotFound;
        }

        if (user.Password is null || !_passwordManager.Verify(request.OldPassword, user.Password))
        {
            return (Error)CustomErrors.User.PasswordNotCorrect;
        }

        user.UpdatePassword(_passwordManager.CreateHashString(request.NewPassword));

        return Result.Success;
    }
}
