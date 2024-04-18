using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Queries.GetCart;

public class GetCartQueryHandler(ICartRepository cartRepository) : IRequestHandler<GetCartQuery, ErrorOr<Cart?>>
{
    private readonly ICartRepository _cartRepository = cartRepository;
    public async Task<ErrorOr<Cart?>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        return await _cartRepository.GetCartByUserIdAsync(UserId.Create(query.UserId));
    }
}