using ErrorOr;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

/// <summary>
/// Represents a command to remove a product from a shopping cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="CartId">The cart's unique identifier.</param>
/// <param name="ItemId">The cart item's unique identifier to remove.</param>
public record RemoveProductFromCartCommand(Guid UserId, Guid CartId, Guid ItemId)
    : MediatR.IRequest<ErrorOr<Success>>;
