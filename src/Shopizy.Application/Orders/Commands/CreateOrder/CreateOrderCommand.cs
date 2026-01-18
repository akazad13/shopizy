using ErrorOr;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Represents a command to create a new order.
/// </summary>
/// <param name="UserId">The user's unique identifier.</param>
/// <param name="PromoCode">The promotional code to apply (if any).</param>
/// <param name="DeliveryMethod">The delivery method identifier.</param>
/// <param name="DeliveryChargeAmount">The delivery charge amount.</param>
/// <param name="DeliveryChargeCurrency">The delivery charge currency.</param>
/// <param name="OrderItems">The collection of order items.</param>
/// <param name="ShippingAddress">The shipping address.</param>
public record CreateOrderCommand(
    Guid UserId,
    string PromoCode,
    int DeliveryMethod,
    decimal DeliveryChargeAmount,
    Currency DeliveryChargeCurrency,
    IEnumerable<OrderItemCommand> OrderItems,
    AddressCommand ShippingAddress
) : MediatR.IRequest<ErrorOr<Order>>;

/// <summary>
/// Represents an order item within a create order command.
/// </summary>
/// <param name="ProductId">The product's unique identifier.</param>
/// <param name="Color">The product color.</param>
/// <param name="Size">The product size.</param>
/// <param name="Quantity">The quantity ordered.</param>
public record OrderItemCommand(Guid ProductId, string Color, string Size, int Quantity);

/// <summary>
/// Represents an address within a create order command.
/// </summary>
/// <param name="Street">The street address.</param>
/// <param name="City">The city.</param>
/// <param name="State">The state or province.</param>
/// <param name="Country">The country.</param>
/// <param name="ZipCode">The postal/ZIP code.</param>
public record AddressCommand(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);
