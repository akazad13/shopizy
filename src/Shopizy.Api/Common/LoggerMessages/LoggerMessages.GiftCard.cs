namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(EventId = 1045, Level = LogLevel.Error, Message = "An error occurred while fetching gift cards.")]
    public static partial void GiftCardFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1046, Level = LogLevel.Error, Message = "An error occurred while creating gift card.")]
    public static partial void GiftCardCreationError(this ILogger logger, Exception ex);
}
