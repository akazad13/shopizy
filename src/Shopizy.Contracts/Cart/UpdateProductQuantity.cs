namespace Shopizy.Contracts.Cart;

public record UpdateProductQuantityRequest(Guid ProductId, int Quantity);
