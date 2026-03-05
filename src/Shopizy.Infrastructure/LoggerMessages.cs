using System;
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
}
