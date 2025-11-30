namespace Shopizy.Contracts.Cart;

/// <summary>
/// Represents a request to add a product to the shopping cart.
/// </summary>
/// <param name="ProductId">The unique identifier of the product to add.</param>
/// <param name="Color">The selected color of the product.</param>
/// <param name="Size">The selected size of the product.</param>
/// <param name="Quantity">The quantity of the product to add.</param>
public record AddProductToCartRequest(Guid ProductId, string Color, string Size, int Quantity);
