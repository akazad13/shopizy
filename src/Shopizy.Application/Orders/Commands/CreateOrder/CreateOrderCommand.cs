using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

[Authorize(Permissions = Permission.Order.Get, Policies = Policy.SelfOrAdmin)]
public record CreateOrderCommand(
    Guid UserId,
    string PromoCode,
    decimal DeliveryChargeAmount,
    Currency DeliveryChargeCurrency,
    List<OrderItemCommand> OrderItems,
    AddressCommand ShippingAddress
) : IAuthorizeableRequest<ErrorOr<Order>>;

public record OrderItemCommand(Guid ProductId, int Quantity);

public record AddressCommand(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);
