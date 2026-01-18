using ErrorOr;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

/// <summary>
/// Represents a command to update the quantity of a product in the cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CartId">The cart's unique identifier.</param>
/// <param name="CartItemId">The cart item's unique identifier.</param>
/// <param name="ProductId">The product's unique identifier.</param>
/// <param name="Quantity">The new quantity.</param>
public record UpdateProductQuantityCommand(
    Guid UserId,
    Guid CartId,
    Guid CartItemId,
    Guid ProductId,
    int Quantity
) : MediatR.IRequest<ErrorOr<Success>>;
