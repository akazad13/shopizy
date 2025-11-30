namespace Shopizy.Contracts.Order;

/// <summary>
/// Represents a summary of an order.
/// </summary>
/// <param name="OrderId">The unique identifier of the order.</param>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Total">The total price of the order.</param>
/// <param name="OrderStatus">The current status of the order.</param>
/// <param name="CreatedOn">The date and time when the order was created.</param>
public record OrderResponse(
    Guid OrderId,
    Guid UserId,
    Price Total,
    string OrderStatus,
    DateTime CreatedOn
);

/// <summary>
/// Represents detailed information about an order.
/// </summary>
/// <param name="OrderId">The unique identifier of the order.</param>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="DeliveryCharge">The delivery charge.</param>
/// <param name="OrderStatus">The current status of the order.</param>
/// <param name="PromoCode">The promo code applied.</param>
/// <param name="ShippingAddress">The shipping address.</param>
/// <param name="PaymentStatus">The payment status.</param>
/// <param name="OrderItems">The list of items in the order.</param>
/// <param name="CreatedOn">The date and time when the order was created.</param>
/// <param name="ModifiedOn">The date and time when the order was last modified.</param>
public record OrderDetailResponse(
    Guid OrderId,
    Guid UserId,
    Price DeliveryCharge,
    string OrderStatus,
    string PromoCode,
    Address ShippingAddress,
    string PaymentStatus,
    IList<OrderItemResponse> OrderItems,
    DateTime CreatedOn,
    DateTime ModifiedOn
);

/// <summary>
/// Represents an item within an order response.
/// </summary>
/// <param name="OrderItemId">The unique identifier of the order item.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="UnitPrice">The unit price of the product.</param>
/// <param name="PictureUrl">The URL of the product image.</param>
/// <param name="Color">The selected color.</param>
/// <param name="Size">The selected size.</param>
/// <param name="Quantity">The quantity ordered.</param>
/// <param name="Discount">The discount applied.</param>
public record OrderItemResponse(
    Guid OrderItemId,
    string Name,
    Price UnitPrice,
    string PictureUrl,
    string Color,
    string Size,
    int Quantity,
    decimal Discount
);
