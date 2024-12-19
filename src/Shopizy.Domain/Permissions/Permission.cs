using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Permissions.ValueObjects;

namespace Shopizy.Domain.Permissions;

public class Permission : AggregateRoot<PermissionId, Guid>
{
    public string Name { get; private set; }

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
