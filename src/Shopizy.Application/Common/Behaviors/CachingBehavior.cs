using MediatR;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Caching;

namespace Shopizy.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for handling request caching.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class CachingBehavior<TRequest, TResponse>(
    ICacheHelper cacheHelper,
    ILogger<CachingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachableRequest
{
    private readonly ICacheHelper _cacheHelper = cacheHelper;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Checking cache for {RequestName} with key {CacheKey}", typeof(TRequest).Name, request.CacheKey);
        
        var cacheResult = await _cacheHelper.GetAsync<TResponse>(request.CacheKey);
        if (cacheResult.Success)
        {
            _logger.LogInformation("Cache hit for {RequestName} with key {CacheKey}", typeof(TRequest).Name, request.CacheKey);
            return cacheResult.Value!;
        }

        _logger.LogInformation("Cache miss for {RequestName} with key {CacheKey}. Fetching from source.", typeof(TRequest).Name, request.CacheKey);
        var response = await next();

        await _cacheHelper.SetAsync(request.CacheKey, response, request.Expiration);

        return response;
    }
}
