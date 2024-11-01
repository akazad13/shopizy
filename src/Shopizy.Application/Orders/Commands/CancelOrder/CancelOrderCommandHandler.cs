using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CancelOrderCommand, IResult<GenericResponse>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<IResult<GenericResponse>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));
        if (order is null)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Order.OrderNotFound]);
        }

        order.CancelOrder(request.Reason);

        _orderRepository.Update(order);

        if (await _orderRepository.Commit(cancellationToken) <= 0)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Order.OrderNotCancelled]);
        }
        return Response<GenericResponse>.SuccessResponese("Successfully canceled the order.");
    }
}
