namespace Shopizy.Contracts.Cart;

public record RemoveProductFromCartRequest(List<Guid> ProductIds);
