namespace Shopizy.SharedKernel.Application.Caching;

/// <summary>
/// Marker interface for commands that should invalidate one or more cache entries
/// after a successful execution.
/// </summary>
public interface IInvalidateCache
{
    /// <summary>
    /// The cache keys to remove after the command's <c>SaveChangesAsync</c> commits.
    /// Return an empty list to skip invalidation.
    /// </summary>
    IReadOnlyCollection<string> CacheKeysToInvalidate { get; }
}
