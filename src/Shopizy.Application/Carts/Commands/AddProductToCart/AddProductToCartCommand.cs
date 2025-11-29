using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

/// <summary>
/// Represents a command to add a product to a shopping cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CartId">The cart's unique identifier.</param>
/// <param name="ProductId">The product's unique identifier.</param>
/// <param name="Color">The selected product color.</param>
/// <param name="Size">The selected product size.</param>
/// <param name="Quantity">The quantity to add.</param>
[Authorize(Permissions = Permissions.Cart.Modify)]
public record AddProductToCartCommand(
    Guid UserId,
    Guid CartId,
    Guid ProductId,
    string Color,
    string Size,
    int Quantity
) : IAuthorizeableRequest<ErrorOr<Cart>>;
