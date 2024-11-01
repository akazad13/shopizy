using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

[Authorize(Permissions = Permissions.User.Modify, Policies = Policy.SelfOrAdmin)]
public record UpdatePasswordCommand(Guid UserId, string OldPassword, string NewPassword)
    : IAuthorizeableRequest<IResult<GenericResponse>>;
