using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

[Authorize(Permissions = Permissions.Cart.Modify, Policies = Policy.SelfOrAdmin)]
public record AddProductToCartCommand(Guid UserId, Guid CartId, Guid ProductId)
    : IAuthorizeableRequest<IResult<Cart>>;
