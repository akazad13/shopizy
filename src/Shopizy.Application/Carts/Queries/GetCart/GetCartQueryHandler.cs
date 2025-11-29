using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Queries.GetCart;

/// <summary>
/// Handles the <see cref="GetCartQuery"/> to retrieve a user's shopping cart.
/// </summary>
public class GetCartQueryHandler(ICartRepository cartRepository)
    : IRequestHandler<GetCartQuery, ErrorOr<Cart>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    /// <summary>
    /// Handles retrieving a user's cart.
    /// </summary>
    /// <param name="query">The get cart query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's cart or an error if not found.</returns>
    public async Task<ErrorOr<Cart>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(UserId.Create(query.UserId));
        if (cart is null)
        {
            return CustomErrors.Cart.CartNotFound;
        }
        return cart;
    }
}
