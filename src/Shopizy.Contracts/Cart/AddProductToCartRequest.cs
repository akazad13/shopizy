namespace Shopizy.Contracts.Cart;

public record AddProductToCartRequest(Guid ProductId, string Color, string Size, int Quantity);
