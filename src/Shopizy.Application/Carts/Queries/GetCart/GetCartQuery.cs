using MediatR;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Queries.GetCart;

[Authorize(Permissions = Permissions.Cart.Get, Policies = Policy.SelfOrAdmin)]
public record GetCartQuery(Guid UserId) : IRequest<IResult<Cart?>>;
