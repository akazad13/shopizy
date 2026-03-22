using Microsoft.Extensions.Logging;

namespace Shopizy.Infrastructure;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "An error occurred while initialising the database."
    )]
    public static partial void DatabaseInitializationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "An error occurred while publishing domain events."
    )]
    public static partial void DomainEventPublishingError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Error, Message = "Error retrieving key {Key} from Redis.")]
    public static partial void RedisGetError(this ILogger logger, Exception ex, string key);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Error, Message = "Error setting key {Key} in Redis.")]
    public static partial void RedisSetError(this ILogger logger, Exception ex, string key);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Error, Message = "Error removing key {Key} from Redis.")]
    public static partial void RedisRemoveError(this ILogger logger, Exception ex, string key);
}
