using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderQuery, ErrorOr<Order>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Order>> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));

        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        return order;
    }
}
