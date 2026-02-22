namespace Shopizy.SharedKernel.Application.Caching;

/// <summary>
/// Provides methods for caching operations.
/// </summary>
public interface ICacheHelper
{
    /// <summary>
    /// Retrieves a cached item by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="key">The key of the cached item.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached item or null if not found.</returns>
    Task<CacheResult<T>> GetAsync<T>(string key);

    /// <summary>
    /// Adds or updates a cached item with the specified key and value.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="value">The value of the item to cache.</param>
    /// <param name="expiration">The optional expiration time for the cached item.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a cached item by its key.
    /// </summary>
    /// <param name="key">The key of the cached item to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key);
}
