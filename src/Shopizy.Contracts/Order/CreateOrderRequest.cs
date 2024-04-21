namespace Shopizy.Contracts.Order;

public record CreateOrderRequest(
    string PromoCode,
    Price DeliveryCharge,
    List<OrderItemRequest> OrderItems,
    Address ShippingAddress
);

public record Price(decimal Amount, int Currency);

public record OrderItemRequest(Guid ProductId, int Quantity);

public record Address(string Line, string City, string State, string Country, string ZipCode);
