namespace Shopizy.Application.Common.Interfaces.Services;

/// <summary>
/// Service interface for dispatching web and mobile push notifications.
/// </summary>
public interface IPushNotificationService
{
    /// <summary>
    /// Sends a push notification to a user's subscribed devices.
    /// </summary>
    Task<bool> SendPushNotificationAsync(
        Guid userId,
        string title,
        string body,
        string? targetUrl = null,
        CancellationToken cancellationToken = default
    );
}
