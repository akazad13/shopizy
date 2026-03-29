namespace Shopizy.Contracts.AuditLog;

public record AuditLogResponse(
    Guid AuditLogId,
    Guid? UserId,
    string Action,
    string EntityName,
    string EntityId,
    string? OldValues,
    string? NewValues,
    DateTime Timestamp
);
