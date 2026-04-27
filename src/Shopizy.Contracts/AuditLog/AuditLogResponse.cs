namespace Shopizy.Contracts.AuditLog;

/// <summary>
/// A single entry in the audit-log read model.
/// </summary>
/// <param name="AuditLogId">Identifier of the audit-log entry.</param>
/// <param name="UserId">Actor (user) who performed the action; null for system-emitted events.</param>
/// <param name="Action">Verb describing what happened (e.g., "OrderCancelled").</param>
/// <param name="EntityName">Aggregate type the action targeted.</param>
/// <param name="EntityId">Identifier of the targeted entity, serialized as a string.</param>
/// <param name="OldValues">JSON snapshot of the entity before the change, when available.</param>
/// <param name="NewValues">JSON snapshot of the entity after the change, when available.</param>
/// <param name="Timestamp">UTC timestamp of when the event was recorded.</param>
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
