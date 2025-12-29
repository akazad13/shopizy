using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

/// <summary>
/// Handles the <see cref="RemoveProductFromCartCommand"/> to remove items from a cart.
/// </summary>
public class RemoveProductFromCartCommandHandler(ICartRepository cartRepository)
    : IRequestHandler<RemoveProductFromCartCommand, ErrorOr<Success>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    /// <summary>
    /// Handles removing a product from the cart.
    /// </summary>
    /// <param name="cmd">The remove product from cart command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success result or an error.</returns>
    public async Task<ErrorOr<Success>> Handle(
        RemoveProductFromCartCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var cart = await _cartRepository.GetCartByIdAsync(
            CartId.Create(cmd.CartId),
            cancellationToken
        );

        if (cart is null)
        {
            return CustomErrors.Cart.CartNotFound;
        }

        var lineItem = cart.CartItems.FirstOrDefault(li => li.Id.Value == cmd.ItemId);
        if (lineItem is not null)
        {
            cart.RemoveLineItem(lineItem);
        }

        _cartRepository.Update(cart);

        return Result.Success;
    }
}
