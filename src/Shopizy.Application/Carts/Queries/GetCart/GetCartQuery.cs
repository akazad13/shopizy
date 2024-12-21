using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Queries.GetCart;

[Authorize(Permissions = Permissions.Cart.Get)]
public record GetCartQuery(Guid UserId) : IAuthorizeableRequest<ErrorOr<Cart>>;
