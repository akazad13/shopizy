using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

[Authorize(Permissions = Permission.Product.Create, Policies = Policy.SelfOrAdmin)]
public record UpdateProductQuantityCommand(Guid UserId, Guid CartId, Guid ProductId, int Quantity)
    : IAuthorizeableRequest<ErrorOr<Success>>;
