using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

/// <summary>
/// Represents a command to update the quantity of a product in the cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CartItemId">The cart item's unique identifier.</param>
/// <param name="Quantity">The new quantity.</param>
public record UpdateProductQuantityCommand(
    Guid UserId,
    Guid CartItemId,
    int Quantity
) : ICommand<ErrorOr<Cart>>;

