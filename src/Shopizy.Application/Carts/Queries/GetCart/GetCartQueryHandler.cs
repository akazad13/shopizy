using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Queries.GetCart;

public class GetCartQueryHandler(ICartRepository cartRepository)
    : IRequestHandler<GetCartQuery, IResult<Cart?>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<IResult<Cart?>> Handle(
        GetCartQuery query,
        CancellationToken cancellationToken
    )
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(UserId.Create(query.UserId));
        return Response<Cart?>.SuccessResponese(cart);
    }
}
