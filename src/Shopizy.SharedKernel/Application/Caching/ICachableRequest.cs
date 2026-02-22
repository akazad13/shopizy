namespace Shopizy.SharedKernel.Application.Caching;

/// <summary>
/// Interface for requests that support caching.
/// </summary>
public interface ICachableRequest
{
    /// <summary>
    /// Gets the cache key for the request.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Gets the expiration time for the cache entry.
    /// </summary>
    TimeSpan? Expiration { get; }
}
