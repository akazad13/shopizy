using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

/// <summary>
/// Represents a command to update a user's password.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="OldPassword">The current password.</param>
/// <param name="NewPassword">The new password.</param>
[Authorize(Permissions = Permissions.User.Modify)]
public record UpdatePasswordCommand(Guid UserId, string OldPassword, string NewPassword)
    : IAuthorizeableRequest<ErrorOr<Success>>;
