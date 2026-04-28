using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderQuery, ErrorOr<Order>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Order>> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));

        if (order is null)
        {
            return (Error)CustomErrors.Order.OrderNotFound;
        }

        return order;
    }
}
