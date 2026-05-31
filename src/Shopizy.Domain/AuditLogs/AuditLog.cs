using Shopizy.Domain.AuditLogs.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.AuditLogs;

public sealed class AuditLog : Entity<AuditLogId>
{
    public Guid? UserId { get; private set; }
    public string Action { get; } = null!;
    public string EntityName { get; } = null!;
    public string EntityId { get; } = null!;
    public string? OldValues { get; }
    public string? NewValues { get; }
    public DateTime Timestamp { get; }

    public static AuditLog Create(
        Guid? userId,
        string action,
        string entityName,
        string entityId,
        string? oldValues,
        string? newValues
    ) => new(AuditLogId.CreateUnique(), userId, action, entityName, entityId, oldValues, newValues);

    private AuditLog(
        AuditLogId id,
        Guid? userId,
        string action,
        string entityName,
        string entityId,
        string? oldValues,
        string? newValues
    )
        : base(id)
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
