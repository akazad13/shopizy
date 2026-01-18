using ErrorOr;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Queries.GetCart;

/// <summary>
/// Represents a query to retrieve a user's shopping cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
public record GetCartQuery(Guid UserId) : MediatR.IRequest<ErrorOr<Cart>>;
