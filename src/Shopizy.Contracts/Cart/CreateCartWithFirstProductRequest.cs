namespace Shopizy.Contracts.Cart;

public record CreateCartWithFirstProductRequest(
    Guid ProductId,
    string Color,
    string Size,
    int Quantity
);
