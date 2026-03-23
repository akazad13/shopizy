using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.AuditLogs.ValueObjects;

public sealed class AuditLogId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private AuditLogId(Guid value)
    {
        Value = value;
    }

    public static AuditLogId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static AuditLogId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
