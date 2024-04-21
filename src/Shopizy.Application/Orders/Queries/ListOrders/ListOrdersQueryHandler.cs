using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.GetOrder;

public class ListOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<ListOrdersQuery, ErrorOr<List<Order>>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<List<Order>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersAsync();
        return orders;
    }
}
