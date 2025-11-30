namespace Shopizy.Contracts.Order;

/// <summary>
/// Represents a request to create a new order.
/// </summary>
/// <param name="PromoCode">The promo code applied to the order.</param>
/// <param name="DeliveryMethod">The selected delivery method.</param>
/// <param name="DeliveryCharge">The delivery charge.</param>
/// <param name="OrderItems">The list of items in the order.</param>
/// <param name="ShippingAddress">The shipping address.</param>
public record CreateOrderRequest(
    string PromoCode,
    int DeliveryMethod,
    Price DeliveryCharge,
    IList<OrderItemRequest> OrderItems,
    Address ShippingAddress
);

/// <summary>
/// Represents a price value.
/// </summary>
/// <param name="Amount">The monetary amount.</param>
/// <param name="Currency">The currency code.</param>
public record Price(decimal Amount, string Currency);

/// <summary>
/// Represents an item in the order creation request.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Color">The selected color.</param>
/// <param name="Size">The selected size.</param>
/// <param name="Quantity">The quantity ordered.</param>
public record OrderItemRequest(Guid ProductId, string Color, string Size, int Quantity);

/// <summary>
/// Represents a physical address.
/// </summary>
/// <param name="Street">The street address.</param>
/// <param name="City">The city name.</param>
/// <param name="State">The state or province.</param>
/// <param name="Country">The country name.</param>
/// <param name="ZipCode">The postal code.</param>
public record Address(string Street, string City, string State, string Country, string ZipCode);
