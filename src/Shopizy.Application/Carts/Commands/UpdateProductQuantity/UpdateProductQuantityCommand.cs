using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

[Authorize(Permissions = Permissions.Cart.Modify, Policies = Policy.SelfOrAdmin)]
public record UpdateProductQuantityCommand(Guid UserId, Guid CartId, Guid ProductId, int Quantity)
    : IAuthorizeableRequest<IResult<GenericResponse>>;
