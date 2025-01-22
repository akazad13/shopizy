namespace Shopizy.Contracts.Order;

public record OrderResponse(
    Guid OrderId,
    Guid UserId,
    Price Total,
    string OrderStatus,
    DateTime CreatedOn
);

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
