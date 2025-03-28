using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

[Authorize(Permissions = Permissions.Cart.Modify)]
public record AddProductToCartCommand(
    Guid UserId,
    Guid CartId,
    Guid ProductId,
    string Color,
    string Size,
    int Quantity
) : IAuthorizeableRequest<ErrorOr<Cart>>;
