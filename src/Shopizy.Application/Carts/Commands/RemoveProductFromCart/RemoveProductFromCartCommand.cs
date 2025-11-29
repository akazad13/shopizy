using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

/// <summary>
/// Represents a command to remove a product from a shopping cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CartId">The cart's unique identifier.</param>
/// <param name="ItemId">The cart item's unique identifier to remove.</param>
[Authorize(Permissions = Permissions.Cart.Delete)]
public record RemoveProductFromCartCommand(Guid UserId, Guid CartId, Guid ItemId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
