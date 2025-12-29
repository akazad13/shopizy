using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CancelOrderCommand, ErrorOr<Success>>
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
            return CustomErrors.Order.OrderNotFound;
        }

        order.CancelOrder(request.Reason);

        _orderRepository.Update(order);

        return Result.Success;
    }
}
