using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Users.Queries.GetUser;

[Authorize(Permissions = Permissions.User.Get, Policies = Policy.Admin)]
public record GetUserQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<User>>;
