namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 1070,
        Level = LogLevel.Error,
        Message = "An error occurred while sending SMS notification."
    )]
    public static partial void SmsDispatchError(this ILogger logger, Exception ex);
}
