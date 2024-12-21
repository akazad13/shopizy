using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

[Authorize(Permissions = Permissions.Cart.Delete)]
public record RemoveProductFromCartCommand(Guid CartId, Guid ItemId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
