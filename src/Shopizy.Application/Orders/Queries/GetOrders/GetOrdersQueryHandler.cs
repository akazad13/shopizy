using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrdersQuery, ErrorOr<List<OrderDto>?>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<List<OrderDto>?>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        var customerId = request.CustomerId is null
            ? null
            : UserId.Create(request.CustomerId.Value);
        var orders = await _orderRepository.GetOrdersAsync(
            customerId,
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
