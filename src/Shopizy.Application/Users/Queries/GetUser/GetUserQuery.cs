using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Users.Queries.GetUser;

[Authorize(Permissions = Permission.User.Get, Policies = Policy.SelfOrAdmin)]
public record GetUserQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<User>>;
