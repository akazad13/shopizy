using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid UserId,
    string PromoCode,
    Price DeliveryCharge,
    List<OrderItemCommand> OrderItems,
    AddressCommand Address
) : IAuthorizeableRequest<ErrorOr<Order>>;

public record OrderItemCommand(Guid ProductId, int Quantity);

public record AddressCommand(
    string Line,
    string City,
    string State,
    string Country,
    string ZipCode
);
