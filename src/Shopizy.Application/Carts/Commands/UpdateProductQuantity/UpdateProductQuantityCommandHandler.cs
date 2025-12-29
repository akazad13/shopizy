using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

/// <summary>
/// Handles the <see cref="UpdateProductQuantityCommand"/> to update item quantities in a cart.
/// </summary>
public class UpdateProductQuantityCommandHandler(ICartRepository cartRepository)
    : IRequestHandler<UpdateProductQuantityCommand, ErrorOr<Success>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    /// <summary>
    /// Handles updating the quantity of a product in the cart.
    /// </summary>
    /// <param name="cmd">The update product quantity command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success result or an error.</returns>
    public async Task<ErrorOr<Success>> Handle(
        UpdateProductQuantityCommand cmd,
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

        cart.UpdateLineItem(CartItemId.Create(cmd.CartItemId), cmd.Quantity);

        _cartRepository.Update(cart);

        return Result.Success;
    }
}
