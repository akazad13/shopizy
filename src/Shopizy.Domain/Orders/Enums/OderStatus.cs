namespace Shopizy.Domain.Orders.Enums;

public enum OrderStatus
{
    Submitted = 1,
    AwaitingValidation = 2,
    StockConfirmed = 3,
    Paid = 4,
    Shipped = 5,
    Cancelled = 6,
}

public enum DeliveryMethods
{
    Free = 0,
    Standard = 1,
    Express = 2,
    Premium = 3,
}
