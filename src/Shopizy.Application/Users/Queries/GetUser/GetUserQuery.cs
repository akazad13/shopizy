using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Users.Queries.GetUser;

[Authorize(Permissions = Permissions.User.Get)]
public record GetUserQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<UserDto>>;
