namespace Shopizy.Contracts.Order;

public record CreateOrderRequest(
    string PromoCode,
    Price DeliveryCharge,
    IList<OrderItemRequest> OrderItems,
    Address ShippingAddress
);

public record Price(decimal Amount, string Currency);

public record OrderItemRequest(Guid ProductId, int Quantity);

public record Address(string Street, string City, string State, string Country, string ZipCode);
