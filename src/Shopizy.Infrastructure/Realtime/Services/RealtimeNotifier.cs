using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Infrastructure.Realtime.Hubs;

namespace Shopizy.Infrastructure.Realtime.Services;

/// <summary>
/// Dispatches real-time events to connected clients via SignalR hubs.
/// </summary>
public class RealtimeNotifier(
    IHubContext<OrderStatusHub> orderHubContext,
    IHubContext<AdminDashboardHub> adminHubContext,
    ILogger<RealtimeNotifier> logger
) : IRealtimeNotifier
{
    private static readonly Action<
        ILogger,
        Guid,
        Guid,
        string,
        Exception?
    > LogOrderStatusDispatched = LoggerMessage.Define<Guid, Guid, string>(
        LogLevel.Information,
        new EventId(1, nameof(SendOrderStatusUpdateAsync)),
        "Dispatched real-time order status update for User: {UserId}, Order: {OrderId}, Status: {Status}"
    );

    private static readonly Action<ILogger, string, Exception?> LogAdminMetricDispatched =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(SendAdminMetricUpdateAsync)),
            "Dispatched real-time admin metric update: {MetricType}"
        );

    private readonly IHubContext<OrderStatusHub> _orderHubContext = orderHubContext;
    private readonly IHubContext<AdminDashboardHub> _adminHubContext = adminHubContext;
    private readonly ILogger<RealtimeNotifier> _logger = logger;

    public async Task SendOrderStatusUpdateAsync(
        Guid userId,
        Guid orderId,
        string orderStatus,
        CancellationToken cancellationToken = default
    )
    {
        await _orderHubContext
            .Clients.Group($"user-{userId}")
            .SendAsync(
                "ReceiveOrderStatusUpdate",
                new
                {
                    OrderId = orderId,
                    Status = orderStatus,
                    TimestampUtc = DateTime.UtcNow,
                },
                cancellationToken
            );

        LogOrderStatusDispatched(_logger, userId, orderId, orderStatus, null);
    }

    public async Task SendAdminMetricUpdateAsync(
        string metricType,
        object data,
        CancellationToken cancellationToken = default
    )
    {
        await _adminHubContext
            .Clients.Group(AdminDashboardHub.AdminGroup)
            .SendAsync(
                "ReceiveMetricUpdate",
                new
                {
                    MetricType = metricType,
                    Data = data,
                    TimestampUtc = DateTime.UtcNow,
                },
                cancellationToken
            );

        LogAdminMetricDispatched(_logger, metricType, null);
    }
}
