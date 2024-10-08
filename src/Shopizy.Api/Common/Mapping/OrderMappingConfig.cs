using Mapster;
using Shopizy.Application.Orders.Commands.CancelOrder;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.Orders.Queries.GetOrder;
using Shopizy.Application.Orders.Queries.ListOrders;
using Shopizy.Contracts.Order;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;

namespace Shopizy.Api.Common.Mapping;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        _ = config
            .NewConfig<(Guid UserId, CreateOrderRequest request), CreateOrderCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request)
            .Map(dest => dest.DeliveryChargeAmount, src => src.request.DeliveryCharge.Amount)
            .Map(
                dest => dest.DeliveryChargeCurrency,
                src => (Currency)src.request.DeliveryCharge.Currency
            );
        _ = config
            .NewConfig<(Guid UserId, Guid OrderId), GetOrderQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.OrderId, src => src.OrderId);

        _ = config
            .NewConfig<Order, OrderResponse>()
            .Map(dest => dest.OrderId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest, src => src)
            .Map(dest => dest.OrderStatus, src => src.OrderStatus.ToString())
            .Map(dest => dest.PaymentStatus, src => src.PaymentStatus.ToString());

        _ = config
            .NewConfig<Guid, ListOrdersQuery>()
            .MapWith(userId => new ListOrdersQuery(userId));

        _ = config
            .NewConfig<OrderItem, OrderItemResponse>()
            .Map(dest => dest.OrderItemId, src => src.Id.Value);

        _ = config
            .NewConfig<
                (Guid UserId, Guid OrderId, CancelOrderRequest request),
                CancelOrderCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.OrderId, src => src.OrderId)
            .Map(dest => dest.Reason, src => src.request.Reason);
    }
}
