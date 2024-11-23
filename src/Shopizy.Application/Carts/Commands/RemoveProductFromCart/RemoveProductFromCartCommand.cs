using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

[Authorize(Permissions = Permissions.Cart.Delete, Policies = Policy.SelfOrAdmin)]
public record RemoveProductFromCartCommand(Guid UserId, Guid CartId, Guid ProductId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
