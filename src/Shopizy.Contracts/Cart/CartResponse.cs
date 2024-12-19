namespace Shopizy.Contracts.Cart;

public record CartResponse(
    Guid CartId,
    Guid UserId,
    DateTime CreatedOn,
    DateTime ModifiedOn,
    IList<CartItemResponse> CartItems
);

public record CartItemResponse(
    Guid CartItemId,
    Guid ProductId,
    string Color,
    string Size,
    int Quantity,
    CartProductResponse Product
);

public record CartProductResponse(
    string Name,
    string Description,
    string Price,
    decimal Discount,
    string Brand,
    int StockQuantity,
    IList<string>? ProductImages
);
