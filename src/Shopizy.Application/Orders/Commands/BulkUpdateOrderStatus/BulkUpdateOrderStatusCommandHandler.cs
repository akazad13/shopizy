using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;

public class BulkUpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<BulkUpdateOrderStatusCommand, ErrorOr<Success>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Success>> Handle(BulkUpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersByIdsAsync(
            request.OrderIds.Select(OrderId.Create).ToList());

        foreach (var order in orders)
        {
            order.UpdateOrderStatus((OrderStatus)request.Status);
        }

        foreach (var order in orders)
        {
            _orderRepository.Update(order);
        }

        return Result.Success;
    }
}
