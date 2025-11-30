namespace Shopizy.Contracts.Cart;

/// <summary>
/// Represents the shopping cart response.
/// </summary>
/// <param name="CartId">The unique identifier of the cart.</param>
/// <param name="UserId">The unique identifier of the user who owns the cart.</param>
/// <param name="CreatedOn">The date and time when the cart was created.</param>
/// <param name="ModifiedOn">The date and time when the cart was last modified.</param>
/// <param name="CartItems">The list of items in the cart.</param>
public record CartResponse(
    Guid CartId,
    Guid UserId,
    DateTime CreatedOn,
    DateTime ModifiedOn,
    IList<CartItemResponse> CartItems
);

/// <summary>
/// Represents an item in the shopping cart.
/// </summary>
/// <param name="CartItemId">The unique identifier of the cart item.</param>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Color">The selected color of the product.</param>
/// <param name="Size">The selected size of the product.</param>
/// <param name="Quantity">The quantity of the product.</param>
/// <param name="Product">The product details.</param>
public record CartItemResponse(
    Guid CartItemId,
    Guid ProductId,
    string Color,
    string Size,
    int Quantity,
    CartProductResponse Product
);

/// <summary>
/// Represents the product details within a cart item.
/// </summary>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The description of the product.</param>
/// <param name="Price">The price of the product.</param>
/// <param name="Discount">The discount applied to the product.</param>
/// <param name="Brand">The brand of the product.</param>
/// <param name="StockQuantity">The available stock quantity.</param>
/// <param name="ProductImages">The list of product image URLs.</param>
public record CartProductResponse(
    string Name,
    string Description,
    decimal Price,
    decimal Discount,
    string Brand,
    int StockQuantity,
    IList<string>? ProductImages
);
