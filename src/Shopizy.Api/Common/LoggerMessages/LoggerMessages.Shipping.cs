namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 1050,
        Level = LogLevel.Error,
        Message = "An error occurred while estimating shipping rates."
    )]
    public static partial void ShippingRateEstimationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1051,
        Level = LogLevel.Error,
        Message = "An error occurred while retrieving order shipment tracking."
    )]
    public static partial void OrderTrackingFetchError(this ILogger logger, Exception ex);
}
