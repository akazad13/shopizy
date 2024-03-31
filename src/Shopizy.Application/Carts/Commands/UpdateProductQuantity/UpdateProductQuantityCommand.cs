using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

[Authorize(Permissions = Permission.Cart.Modify, Policies = Policy.SelfOrAdmin)]
public record UpdateProductQuantityCommand(Guid UserId, Guid CartId, Guid ProductId, int Quantity)
    : IAuthorizeableRequest<ErrorOr<Success>>;
