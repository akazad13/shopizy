using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Queries.GetShipment;

public class GetShipmentQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetShipmentQuery, ErrorOr<Shipment>>
{
    public async Task<ErrorOr<Shipment>> Handle(
        GetShipmentQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var order = await orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        if (order.UserId != UserId.Create(request.UserId))
        {
            return Error.Forbidden("Order.Forbidden", "You are not authorized to access this order.");
        }

        if (order.Shipment is null)
        {
            return Error.NotFound("Order.ShipmentNotFound", "Order has no shipment.");
        }

        return order.Shipment;
    }
}
