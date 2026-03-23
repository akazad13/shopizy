using Shopizy.Domain.AuditLogs.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.AuditLogs;

public sealed class AuditLog : Entity<AuditLogId>
{
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string EntityName { get; private set; } = null!;
    public string EntityId { get; private set; } = null!;
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public DateTime Timestamp { get; private set; }

    public static AuditLog Create(
        Guid? userId,
        string action,
        string entityName,
        string entityId,
        string? oldValues,
        string? newValues
    )
    {
        return new AuditLog(AuditLogId.CreateUnique(), userId, action, entityName, entityId, oldValues, newValues);
    }

    private AuditLog(
        AuditLogId id,
        Guid? userId,
        string action,
        string entityName,
        string entityId,
        string? oldValues,
        string? newValues
    ) : base(id)
    {
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        Timestamp = DateTime.UtcNow;
    }

    private AuditLog() { }
}
