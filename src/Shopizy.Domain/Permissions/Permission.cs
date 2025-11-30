using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Permissions.ValueObjects;

namespace Shopizy.Domain.Permissions;

/// <summary>
/// Represents a permission in the authorization system.
/// </summary>
public class Permission : AggregateRoot<PermissionId, Guid>
{
    /// <summary>
    /// Gets the permission name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="name">The permission name.</param>
    /// <returns>A new <see cref="Permission"/> instance.</returns>
    public static Permission Create(string name)
    {
        return new Permission(PermissionId.CreateUnique(), name);
    }

    private Permission(PermissionId permissionId, string name)
        : base(permissionId)
    {
        Name = name;
    }

    private Permission() { }
}
