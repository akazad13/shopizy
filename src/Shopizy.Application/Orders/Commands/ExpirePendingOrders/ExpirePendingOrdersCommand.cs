using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.ExpirePendingOrders;

/// <summary>
/// Command to find and cancel pending orders that have exceeded the payment timeout window.
/// </summary>
/// <param name="ThresholdUtc">Orders created prior to this timestamp will be cancelled.</param>
/// <param name="MaxBatchSize">Maximum number of orders to process per execution run.</param>
public sealed record ExpirePendingOrdersCommand(DateTime ThresholdUtc, int MaxBatchSize = 50)
    : ICommand<ErrorOr<int>>;
