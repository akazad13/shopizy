using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Queries.GetCart;

public class GetCartQueryHandler(ICartRepository cartRepository)
    : IRequestHandler<GetCartQuery, ErrorOr<Cart>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

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
