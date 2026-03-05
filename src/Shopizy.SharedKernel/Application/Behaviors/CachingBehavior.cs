using ErrorOr;
using Microsoft.Extensions.Logging;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Caching;

namespace Shopizy.SharedKernel.Application.Behaviors;

public class CachingQueryHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> innerHandler,
    ICacheHelper cacheHelper,
    ILogger<CachingQueryHandlerDecorator<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>, ICachableRequest
{
    private readonly IQueryHandler<TQuery, TResponse> _innerHandler = innerHandler;
    private readonly ICacheHelper _cacheHelper = cacheHelper;
    private readonly ILogger<CachingQueryHandlerDecorator<TQuery, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogCheckingCache(typeof(TQuery).Name, query.CacheKey);

        var cacheResult = await _cacheHelper.GetAsync<TResponse>(query.CacheKey);
        if (cacheResult.Success)
        {
            _logger.LogCacheHit(typeof(TQuery).Name, query.CacheKey);
            return cacheResult.Value!;
        }

        _logger.LogCacheMiss(typeof(TQuery).Name, query.CacheKey);
        var response = await _innerHandler.Handle(query, cancellationToken);
        await _cacheHelper.SetAsync(query.CacheKey, response, query.Expiration);

        return response;
    }
}
