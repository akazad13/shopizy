using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<UpdateOrderStatusCommand, ErrorOr<Success>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Success>> Handle(
        UpdateOrderStatusCommand command,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(command);

        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(command.OrderId));
        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        order.UpdateOrderStatus(command.Status);
        _orderRepository.Update(order);

        return Result.Success;
    }
}
