using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

[Authorize(Permissions = Permissions.User.Modify)]
public record UpdatePasswordCommand(Guid UserId, string OldPassword, string NewPassword)
    : IAuthorizeableRequest<ErrorOr<Success>>;
