namespace Shopizy.Application.Common.Caching;

/// <summary>
/// Represents the result of a cache retrieval operation.
/// </summary>
/// <typeparam name="T">The type of the cached value.</typeparam>
public class CacheResult<T>
{
    private CacheResult(bool success, T? value)
    {
        Success = success;
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the cache operation was successful (item was found).
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the cached value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Creates a successful cache result.
    /// </summary>
    /// <param name="value">The cached value.</param>
    /// <returns>A successful cache result.</returns>
    public static CacheResult<T> Hit(T value) => new(true, value);

    /// <summary>
    /// Creates a failed cache result (cache miss).
    /// </summary>
    /// <returns>A failed cache result.</returns>
    public static CacheResult<T> Miss() => new(false, default);
}
