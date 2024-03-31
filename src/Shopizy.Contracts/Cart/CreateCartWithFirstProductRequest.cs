namespace Shopizy.Contracts.Cart;

public record CreateCartWithFirstProductRequest(Guid CustomerId, Guid ProductId);
