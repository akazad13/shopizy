using Shopizy.Domain.Permissions.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

/// <summary>
/// Resolves <see cref="PermissionId"/> values from permission names with cached lookups.
/// </summary>
public interface IPermissionLookup
{
    /// <summary>
    /// Returns the <see cref="PermissionId"/> values for the supplied permission names.
    /// Unknown names are skipped silently — pass a curated whitelist.
    /// </summary>
    /// <param name="names"></param>
    /// <param name="cancellationToken"></param>
    Task<IReadOnlyList<PermissionId>> GetIdsByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Drops the in-memory cache so the next call refreshes from the database.
    /// </summary>
    void Invalidate();
}
