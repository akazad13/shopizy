using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.CreateShipment;

public class CreateShipmentCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<CreateShipmentCommand, ErrorOr<Shipment>>
{
    public async Task<ErrorOr<Shipment>> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var order = await orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        var result = order.AddShipment(
            request.Carrier,
            request.TrackingNumber,
            request.EstimatedDelivery
        );

        if (result.IsError) return result.Errors;

        orderRepository.Update(order);

        return result.Value;
    }
}
