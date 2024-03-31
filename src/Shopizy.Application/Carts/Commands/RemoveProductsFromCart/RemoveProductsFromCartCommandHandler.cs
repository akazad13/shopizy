using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Carts.Commands.RemoveProductsFromCart;

public class RemoveProductFromCartCommandHandler(ICartRepository _cartRepository)
        : IRequestHandler<RemoveProductFromCartCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RemoveProductFromCartCommand cmd, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetCartByIdAsync(CartId.Create(cmd.CartId));

        if(cart is null)
            return CustomErrors.Cart.CartNotFound;
        
        var lineItems = cart.LineItems.Where(li => cmd.ProductIds.Any(p => p.Equals(li.ProductId))).Select(li => li);

        foreach (var lineItem in lineItems)
        {
            cart.RemoveLineItem(lineItem);
        }

        _cartRepository.Update(cart);
        
        if (await _cartRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Cart.CartPrductNotRemoved;
        return Result.Success;

    }
}