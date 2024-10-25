using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

[Authorize(Permissions = Permissions.Cart.Delete, Policies = Policy.SelfOrAdmin)]
public record RemoveProductFromCartCommand(Guid UserId, Guid CartId, Guid ProductId)
    : IAuthorizeableRequest<IResult<GenericResponse>>;
