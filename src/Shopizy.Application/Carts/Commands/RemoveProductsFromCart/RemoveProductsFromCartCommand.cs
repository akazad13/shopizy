using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;

namespace Shopizy.Application.Carts.Commands.RemoveProductsFromCart;

[Authorize(Permissions = Permission.Cart.Delete, Policies = Policy.SelfOrAdmin)]
public record RemoveProductFromCartCommand(Guid UserId, Guid CartId, List<Guid> ProductIds)
    : IAuthorizeableRequest<ErrorOr<Success>>;
