using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

[Authorize(Permissions = Permissions.Cart.Modify)]
public record UpdateProductQuantityCommand(
    Guid UserId,
    Guid CartId,
    Guid CartItemId,
    Guid ProductId,
    int Quantity
) : IAuthorizeableRequest<ErrorOr<Success>>;
