using System.Globalization;

namespace Shopizy.Application;

/// <summary>
/// Stable cache-key formats so query-side caching and command-side invalidation stay in sync.
/// </summary>
public static class CacheKeys
{
    public static string Product(Guid productId) =>
        string.Create(CultureInfo.InvariantCulture, $"product:{productId}");
}
