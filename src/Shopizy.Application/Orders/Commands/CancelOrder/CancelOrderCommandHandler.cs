using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : ICommandHandler<CancelOrderCommand, ErrorOr<Success>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Success>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
        if (order is null)
        {
            return (Error)CustomErrors.Order.OrderNotFound;
        }

        order.CancelOrder(request.Reason);

        return Result.Success;
    }
}
