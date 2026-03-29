namespace Shopizy.Domain.Orders.Enums;

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Shipping = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6,
}

public enum DeliveryMethods
{
    Free = 0,
    Standard = 1,
    Express = 2,
    Premium = 3,
}
