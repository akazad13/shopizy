namespace Shopizy.Contracts.Cart;

/// <summary>
/// Represents a request to update the quantity of a product in the cart.
/// </summary>
/// <param name="Quantity">The new quantity.</param>
public record UpdateProductQuantityRequest(int Quantity);
