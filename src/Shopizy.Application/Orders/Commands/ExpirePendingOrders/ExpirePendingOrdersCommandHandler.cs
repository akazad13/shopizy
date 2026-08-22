using ErrorOr;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.ExpirePendingOrders;

/// <summary>
/// Handler that cancels expired pending orders and saves changes, triggering domain events to restore stock.
/// </summary>
public sealed class ExpirePendingOrdersCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<ExpirePendingOrdersCommandHandler> logger
) : ICommandHandler<ExpirePendingOrdersCommand, ErrorOr<int>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ExpirePendingOrdersCommandHandler> _logger = logger;

    public async Task<ErrorOr<int>> Handle(
        ExpirePendingOrdersCommand request,
        CancellationToken cancellationToken
    )
    {
        var expiredOrders = await _orderRepository.GetExpiredPendingOrdersAsync(
            request.ThresholdUtc,
            request.MaxBatchSize,
            cancellationToken
        );

        if (expiredOrders.Count == 0)
        {
            return 0;
        }

        foreach (var order in expiredOrders)
        {
            order.CancelOrder("Order expired due to non-payment within the checkout window.");
            _orderRepository.Update(order);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return expiredOrders.Count;
    }
}
