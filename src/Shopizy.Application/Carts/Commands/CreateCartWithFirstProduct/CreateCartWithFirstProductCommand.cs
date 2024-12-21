using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;

[Authorize(Permissions = Permissions.Cart.Create)]
public record CreateCartWithFirstProductCommand(Guid ProductId, string Color, string Size)
    : IAuthorizeableRequest<ErrorOr<Cart>>;
