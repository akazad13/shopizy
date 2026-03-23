using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.AddShipment;

public class AddShipmentCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<AddShipmentCommand, ErrorOr<Shipment>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Shipment>> Handle(
        AddShipmentCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
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

        _orderRepository.Update(order);

        return result.Value;
    }
}
