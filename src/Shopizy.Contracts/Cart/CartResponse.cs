namespace Shopizy.Contracts.Cart;

public record CartResponse(
    Guid CartId,
    Guid CustomerId,
    DateTime CreatedOn,
    DateTime ModifiedOn,
    List<LineItemResponse> LineItems
);

public record LineItemResponse(
    Guid LineItemId,
    Guid ProductId,
    string Name,
    string Description,
    string Price,
    decimal Discount,
    string Brand,
    int StockQuantity,
    List<string> ProductImages
);
