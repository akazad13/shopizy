using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Users.Queries.GetUser;

/// <summary>
/// Represents a query to retrieve user information by user ID.
/// </summary>
/// <param name="UserId">The unique identifier of the user to retrieve.</param>
[Authorize(Permissions = Permissions.User.Get)]
public record GetUserQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<UserDto>>;
