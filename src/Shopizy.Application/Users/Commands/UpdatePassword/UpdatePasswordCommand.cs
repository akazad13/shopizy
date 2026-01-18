using ErrorOr;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

/// <summary>
/// Represents a command to update a user's password.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="OldPassword">The current password.</param>
/// <param name="NewPassword">The new password.</param>
public record UpdatePasswordCommand(Guid UserId, string OldPassword, string NewPassword)
    : MediatR.IRequest<ErrorOr<Success>>;
