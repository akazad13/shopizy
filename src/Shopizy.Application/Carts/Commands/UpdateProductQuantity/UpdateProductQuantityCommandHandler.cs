using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

/// <summary>
/// Handles the <see cref="UpdateProductQuantityCommand"/> to update item quantities in a cart.
/// </summary>
public class UpdateProductQuantityCommandHandler(ICartRepository cartRepository)
    : ICommandHandler<UpdateProductQuantityCommand, ErrorOr<Cart>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    /// <summary>
    /// Handles updating the quantity of a product in the cart.
    /// </summary>
    /// <param name="cmd">The update product quantity command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated cart or an error.</returns>
    public async Task<ErrorOr<Cart>> Handle(
        UpdateProductQuantityCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var userId = UserId.Create(cmd.UserId);
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cart is null)
        {
            return CustomErrors.Cart.CartNotFound;
        }

        cart.UpdateLineItem(CartItemId.Create(cmd.CartItemId), cmd.Quantity);

        _cartRepository.Update(cart);

        return await _cartRepository.GetCartByUserIdAsync(userId);
    }
}
