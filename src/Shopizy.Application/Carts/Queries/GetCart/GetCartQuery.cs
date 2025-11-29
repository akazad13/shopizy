using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Queries.GetCart;

/// <summary>
/// Represents a query to retrieve a user's shopping cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
[Authorize(Permissions = Permissions.Cart.Get)]
public record GetCartQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<Cart>>;
