using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;

[Authorize(Permissions = Permission.Cart.Create, Policies = Policy.SelfOrAdmin)]
public record CreateCartWithFirstProductCommand(Guid UserId, Guid CustomerId, Guid ProductId)
    : IAuthorizeableRequest<ErrorOr<Cart>>;
