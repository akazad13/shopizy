using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

/// <summary>
/// Represents a command to remove a product from a shopping cart.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="ItemId">The cart item's unique identifier to remove.</param>
public record RemoveProductFromCartCommand(Guid UserId, Guid ItemId)
    : ICommand<ErrorOr<Cart>>;

