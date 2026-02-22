using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Orders.Queries.GetOrders;
using Shopizy.Contracts.Order;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for order-related models.
/// </summary>
public class OrderMappingConfig : IRegister
{
    /// <summary>
    /// Registers the mapping configurations.
    /// </summary>
    /// <param name="config">The type adapter configuration.</param>
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config.NewConfig<Shopizy.Domain.Common.ValueObjects.Price, Shopizy.Contracts.Order.Price>()
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.Currency, src => src.Currency.ToString());

        config.NewConfig<Shopizy.Domain.Orders.ValueObjects.Address, Shopizy.Contracts.Order.Address>()
            .Map(dest => dest, src => src);

        config
            .NewConfig<(Guid UserId, OrdersCriteria criteria), GetOrdersQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.criteria);

        config
            .NewConfig<(Guid UserId, CreateOrderRequest request), CreateOrderCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.DeliveryChargeAmount, src => src.request.DeliveryCharge.Amount)
            .Map(
                dest => dest.DeliveryChargeCurrency,
                src =>
                    src.request.DeliveryCharge.Currency.ToLower() == "usd" ? Currency.usd
                    : src.request.DeliveryCharge.Currency.ToLower() == "bdt" ? Currency.bdt
                    : src.request.DeliveryCharge.Currency.ToLower() == "euro" ? Currency.euro
                    : Currency.usd
            )
            .Map(dest => dest.OrderItems, src => src.request.OrderItems)
            .Map(dest => dest.ShippingAddress, src => src.request.ShippingAddress);

        config.NewConfig<(Guid UserId, Guid OrderId) , GetOrderQuery>()
            .Map(dest=> dest.UserId, src => src.UserId)
            .Map(dest=> dest.OrderId, src => src.OrderId);

        config
            .NewConfig<Order, OrderDetailResponse>()
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.OrderId, src => src.Id.Value)
            .Map(dest => dest.DeliveryCharge, src => src.DeliveryCharge)
            .Map(dest => dest.ShippingAddress, src => src.ShippingAddress)
            .Map(dest => dest.OrderStatus, src => src.OrderStatus.ToString())
            .Map(dest => dest.PaymentStatus, src => src.PaymentStatus.ToString())
            .Map(dest => dest.ModifiedOn, src => src.ModifiedOn ?? src.CreatedOn);

        config
            .NewConfig<OrderItem, OrderItemResponse>()
            .Map(dest => dest.OrderItemId, src => src.Id.Value);

        config
            .NewConfig<(Guid UserId, Guid OrderId, CancelOrderRequest request), CancelOrderCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.Reason, src => src.request.Reason);

        config
            .NewConfig<OrderDto, OrderResponse>()
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.OrderId, src => src.Id.Value)
            .Map(dest => dest, src => src)
            .Map(dest => dest.OrderStatus, src => src.OrderStatus.ToString());
    }
}
