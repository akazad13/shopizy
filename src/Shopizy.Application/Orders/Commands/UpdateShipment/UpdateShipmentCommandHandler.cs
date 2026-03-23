using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.UpdateShipment;

public class UpdateShipmentCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<UpdateShipmentCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        UpdateShipmentCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var order = await orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        var result = order.UpdateShipment(
            request.Carrier,
            request.TrackingNumber,
            request.EstimatedDelivery,
            (ShipmentStatus)request.Status
        );

        if (result.IsError) return result.Errors;

        orderRepository.Update(order);

        return Result.Success;
    }
}
