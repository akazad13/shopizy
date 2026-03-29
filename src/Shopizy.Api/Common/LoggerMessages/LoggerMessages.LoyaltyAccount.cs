namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(EventId = 1042, Level = LogLevel.Error, Message = "An error occurred while fetching loyalty account.")]
    public static partial void LoyaltyAccountFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1043, Level = LogLevel.Error, Message = "An error occurred while earning loyalty points.")]
    public static partial void LoyaltyPointsEarnError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1044, Level = LogLevel.Error, Message = "An error occurred while redeeming loyalty points.")]
    public static partial void LoyaltyPointsRedeemError(this ILogger logger, Exception ex);
}
