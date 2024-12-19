using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Permissions.ValueObjects;

public sealed class PermissionId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private PermissionId(Guid value)
    {
        Value = value;
    }

    public static PermissionId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static PermissionId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
