namespace Shopizy.Contracts.Order;

public record CreateOrderRequest(
    string PromoCode,
    Price DelivaryCharge,
    List<OrderItem> OrderItems,
    Address ShippingAddress
);

public record Price(decimal Amount, int Currency);

public record OrderItem(string ProductId, int Quantity);

public record Address(string Line, string City, string State, string Country, string ZipCode);
