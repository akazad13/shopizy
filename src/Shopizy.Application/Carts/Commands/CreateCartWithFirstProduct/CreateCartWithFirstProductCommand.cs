using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;

[Authorize(Permissions = Permissions.Cart.Create, Policies = Policy.SelfOrAdmin)]
public record CreateCartWithFirstProductCommand(Guid UserId, Guid ProductId)
    : IAuthorizeableRequest<IResult<Cart>>;
