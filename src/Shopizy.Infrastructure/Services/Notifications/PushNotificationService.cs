using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Infrastructure.Services.Notifications;

public class PushNotificationService(ILogger<PushNotificationService> logger)
    : IPushNotificationService
{
    private static readonly Action<ILogger, Guid, string, Exception?> LogPushSent =
        LoggerMessage.Define<Guid, string>(
            LogLevel.Information,
            new EventId(1, nameof(SendPushNotificationAsync)),
            "Dispatched Push notification to User: {UserId}, Title: '{Title}'"
        );

    private readonly ILogger<PushNotificationService> _logger = logger;

    public Task<bool> SendPushNotificationAsync(
        Guid userId,
        string title,
        string body,
        string? targetUrl = null,
        CancellationToken cancellationToken = default
    )
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(title))
        {
            return Task.FromResult(false);
        }

        LogPushSent(_logger, userId, title, null);
        return Task.FromResult(true);
    }
}
