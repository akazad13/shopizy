using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;

[Authorize(Permissions = Permission.Product.Create, Policies = Policy.SelfOrAdmin)]
public record CreateCartWithFirstProductCommand(
    Guid UserId,
    Guid CustomerId,
    Guid ProductId,
    int Quantity
) : IAuthorizeableRequest<ErrorOr<Cart>>;
