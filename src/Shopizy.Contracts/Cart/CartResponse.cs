namespace Shopizy.Contracts.Cart;

public record CartResponse(
    Guid CartId,
    Guid UserId,
    DateTime CreatedOn,
    DateTime ModifiedOn,
    IList<LineItemResponse> LineItems
);

public record LineItemResponse(
    Guid LineItemId,
    Guid ProductId,
    int Quantity,
    LineProductResponse Product
);

public record LineProductResponse(
    string Name,
    string Description,
    string Price,
    decimal Discount,
    string Brand,
    int StockQuantity,
    IList<string>? ProductImages
);
