using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Commands.RequestReturn;

public class RequestReturnCommandHandler(
    IOrderRepository orderRepository,
    IReturnRequestRepository returnRequestRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<RequestReturnCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        RequestReturnCommand request,
        CancellationToken cancellationToken
    )
    {
        var orderId = OrderId.Create(request.OrderId);
        var userId = UserId.Create(request.UserId);

        var order = await orderRepository.GetOrderByIdAsync(orderId);
        if (order is null || order.UserId != userId)
        {
            return (Error)CustomErrors.Order.OrderNotFound;
        }

        var returnItems = request
            .Items.Select(i => ReturnItem.Create(OrderItemId.Create(i.OrderItemId), i.Quantity))
            .ToList();

        var returnRequest = ReturnRequest.Create(orderId, userId, request.Reason, returnItems);

        await returnRequestRepository.AddAsync(returnRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return returnRequest.Id.Value;
    }
}
