namespace Shopizy.Contracts.Cart;

/// <summary>
/// Represents a request to create a new cart with an initial product.
/// </summary>
/// <param name="ProductId">The unique identifier of the product to add.</param>
/// <param name="Color">The selected color of the product.</param>
/// <param name="Size">The selected size of the product.</param>
/// <param name="Quantity">The quantity of the product to add.</param>
public record CreateCartWithFirstProductRequest(
    Guid ProductId,
    string Color,
    string Size,
    int Quantity
);
