using Microsoft.Extensions.Logging;

namespace Shopizy.SharedKernel.Application;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "Checking cache for {RequestName} with key {CacheKey}")]
    public static partial void LogCheckingCache(this ILogger logger, string requestName, string cacheKey);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Cache hit for {RequestName} with key {CacheKey}")]
    public static partial void LogCacheHit(this ILogger logger, string requestName, string cacheKey);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Cache miss for {RequestName} with key {CacheKey}. Fetching from source.")]
    public static partial void LogCacheMiss(this ILogger logger, string requestName, string cacheKey);
}
