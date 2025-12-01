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
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config
            .NewConfig<(Guid UserId, OrdersCriteria criteria), GetOrdersQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.criteria);

        config
            .NewConfig<(Guid UserId, CreateOrderRequest OrderRequest), CreateOrderCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.DeliveryChargeAmount, src => src.OrderRequest.DeliveryCharge.Amount)
            .Map(
                dest => dest.DeliveryChargeCurrency,
                src =>
                    src.OrderRequest.DeliveryCharge.Currency == "usd" ? Currency.usd
                    : src.OrderRequest.DeliveryCharge.Currency == "bdt" ? Currency.bdt
                    : src.OrderRequest.DeliveryCharge.Currency == "euro" ? Currency.euro
                    : Currency.usd
            );

        config.NewConfig<(Guid UserId, Guid OrderId) , GetOrderQuery>()
            .Map(dest=> dest.UserId, src => src.UserId);

        config
            .NewConfig<Order, OrderDetailResponse>()
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.OrderId, src => src.Id.Value)
            .Map(dest => dest, src => src)
            .Map(dest => dest.OrderStatus, src => src.OrderStatus.ToString())
            .Map(dest => dest.PaymentStatus, src => src.PaymentStatus.ToString());

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
