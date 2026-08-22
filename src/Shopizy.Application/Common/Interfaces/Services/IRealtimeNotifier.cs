namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for dispatching real-time notifications via SignalR.
/// </summary>
public interface IRealtimeNotifier
{
    /// <summary>
    /// Sends a real-time order status update notification to the user.
    /// </summary>
    Task SendOrderStatusUpdateAsync(
        Guid userId,
        Guid orderId,
        string orderStatus,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Broadcasts real-time admin metric updates to connected administrators.
    /// </summary>
    Task SendAdminMetricUpdateAsync(
        string metricType,
        object data,
        CancellationToken cancellationToken = default
    );
}
