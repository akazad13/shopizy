using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Customers.ValueObjects;

namespace Shopizy.Application.Carts.Queries.GetCart;

public class GetCartQueryHandler(ICartRepository _cartRepository) : IRequestHandler<GetCartQuery, ErrorOr<Cart?>>
{
    public async Task<ErrorOr<Cart?>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        return await _cartRepository.GetCartByCustomerIdAsync(CustomerId.Create(query.CustomerId));
    }
}