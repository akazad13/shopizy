namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(EventId = 1050, Level = LogLevel.Error, Message = "An error occurred while fetching audit logs.")]
    public static partial void AuditLogFetchError(this ILogger logger, Exception ex);
}
