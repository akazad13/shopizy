using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

[Authorize(Permissions = Permissions.Order.Create, Policies = Policy.Admin)]
public record CreateOrderCommand(
    string PromoCode,
    int DeliveryMethod,
    decimal DeliveryChargeAmount,
    Currency DeliveryChargeCurrency,
    IEnumerable<OrderItemCommand> OrderItems,
    AddressCommand ShippingAddress
) : IAuthorizeableRequest<ErrorOr<Order>>;

public record OrderItemCommand(Guid ProductId, string Color, string Size, int Quantity);

public record AddressCommand(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);
