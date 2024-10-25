using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Orders;

namespace Shopizy.Application.Orders.Queries.ListOrders;

public class ListOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<ListOrdersQuery, IResult<List<Order>?>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<IResult<List<Order>?>> Handle(
        ListOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        var orders = await _orderRepository.GetOrdersAsync();

        return Response<List<Order>?>.SuccessResponese(orders);
    }
}
