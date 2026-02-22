using MediatR;
using Microsoft.Extensions.Logging;
using Shopizy.SharedKernel.Application.Caching;

namespace Shopizy.SharedKernel.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior for handling request caching.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public partial class CachingBehavior<TRequest, TResponse>(
    ICacheHelper cacheHelper,
    ILogger<CachingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachableRequest
{
    private readonly ICacheHelper _cacheHelper = cacheHelper;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

    [LoggerMessage(Level = LogLevel.Information, Message = "Checking cache for {RequestName} with key {CacheKey}")]
    static partial void LogCheckingCache(ILogger logger, string requestName, string cacheKey);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cache hit for {RequestName} with key {CacheKey}")]
    static partial void LogCacheHit(ILogger logger, string requestName, string cacheKey);

    [LoggerMessage(Level = LogLevel.Information, Message = "Cache miss for {RequestName} with key {CacheKey}. Fetching from source.")]
    static partial void LogCacheMiss(ILogger logger, string requestName, string cacheKey);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        LogCheckingCache(_logger, typeof(TRequest).Name, request.CacheKey);
        
        var cacheResult = await _cacheHelper.GetAsync<TResponse>(request.CacheKey);
        if (cacheResult.Success)
        {
            LogCacheHit(_logger, typeof(TRequest).Name, request.CacheKey);
            return cacheResult.Value!;
        }

        LogCacheMiss(_logger, typeof(TRequest).Name, request.CacheKey);
#pragma warning disable CA2016
        var response = await next();
#pragma warning restore CA2016

        await _cacheHelper.SetAsync(request.CacheKey, response, request.Expiration);

        return response;
    }
}
