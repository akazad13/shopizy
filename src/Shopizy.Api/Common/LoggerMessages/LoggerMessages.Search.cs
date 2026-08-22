namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 1060,
        Level = LogLevel.Error,
        Message = "An error occurred while executing faceted product search."
    )]
    public static partial void ProductSearchError(this ILogger logger, Exception ex);
}
