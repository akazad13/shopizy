using ErrorOr;
using MediatR;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Queries.GetCart;

public record GetCartQuery(Guid CustomerId) : IRequest<ErrorOr<Cart?>>;
