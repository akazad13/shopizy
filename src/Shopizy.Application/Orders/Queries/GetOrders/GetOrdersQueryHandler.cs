using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrdersQuery, ErrorOr<PagedResult<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<PagedResult<OrderDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var userId = request.UserId.HasValue ? UserId.Create(request.UserId.Value) : null;

        var orders = await _orderRepository.GetOrdersAsync(
            userId,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.PageNumber,
            request.PageSize
        );

        var totalCount = await _orderRepository.GetOrdersCountAsync(
            userId,
            request.StartDate,
            request.EndDate,
            request.Status
        );

        var orderList = orders
            .Select(o => new OrderDto(o.Id, o.UserId, o.GetTotal(), o.OrderStatus, o.CreatedOn))
            .ToList();

        return new PagedResult<OrderDto>(
            orderList,
            request.PageNumber,
            request.PageSize,
            totalCount
        );
    }
}
