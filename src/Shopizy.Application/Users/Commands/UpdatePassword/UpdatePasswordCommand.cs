using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

[Authorize(Permissions = Permissions.User.Modify, Policies = Policy.Admin)]
public record UpdatePasswordCommand(Guid UserId, string OldPassword, string NewPassword)
    : IAuthorizeableRequest<ErrorOr<Success>>;
