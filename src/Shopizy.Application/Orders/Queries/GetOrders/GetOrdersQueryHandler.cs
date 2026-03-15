using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrdersQuery, ErrorOr<List<OrderDto>?>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<List<OrderDto>?>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var orders = await _orderRepository.GetOrdersAsync(
            request.UserId.HasValue ? UserId.Create(request.UserId.Value) : null,
            request.StartDate,
            request.EndDate,
            request.Status,
            request.PageNumber,
            request.PageSize
        );

        if (orders is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        var orderList = new List<OrderDto>();

        foreach (var order in orders)
        {
            orderList.Add(
                new OrderDto(
                    order.Id,
                    order.UserId,
                    order.GetTotal(),
                    order.OrderStatus,
                    order.CreatedOn
                )
            );
        }

        return orderList;
    }
}
