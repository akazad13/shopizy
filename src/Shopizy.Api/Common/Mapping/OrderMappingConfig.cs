using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Contracts.Order;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;

namespace Shopizy.Api.Common.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config
            .NewConfig<CreateOrderRequest, CreateOrderCommand>()
            .Map(dest => dest.DeliveryChargeAmount, src => src.DeliveryCharge.Amount)
            .Map(
                dest => dest.DeliveryChargeCurrency,
                src =>
                    src.DeliveryCharge.Currency == "usd" ? Currency.usd
                    : src.DeliveryCharge.Currency == "bdt" ? Currency.bdt
                    : src.DeliveryCharge.Currency == "euro" ? Currency.euro
                    : Currency.usd
            );

        config.NewConfig<Guid, GetOrderQuery>().MapWith(orderId => new GetOrderQuery(orderId));

        config
            .NewConfig<Order, OrderResponse>()
            .Map(dest => dest.OrderId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest, src => src)
            .Map(dest => dest.OrderStatus, src => src.OrderStatus.ToString())
            .Map(dest => dest.PaymentStatus, src => src.PaymentStatus.ToString());

        config
            .NewConfig<OrderItem, OrderItemResponse>()
            .Map(dest => dest.OrderItemId, src => src.Id.Value);

        config
            .NewConfig<(Guid OrderId, CancelOrderRequest request), CancelOrderCommand>()
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.Reason, src => src.request.Reason);
    }
}
