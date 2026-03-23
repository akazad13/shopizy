using Shopizy.Domain.AuditLogs;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLog>> GetLogsAsync(string? entityName, string? entityId, int pageNumber, int pageSize);
    Task AddAsync(AuditLog auditLog);
}
