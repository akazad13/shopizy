using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.AuditLogs;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.AuditLogs.Persistence;

public class AuditLogRepository(AppDbContext dbContext) : IAuditLogRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<AuditLog>> GetLogsAsync(
        string? entityName,
        string? entityId,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext.Set<AuditLog>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(al => al.EntityName == entityName);
        }

        if (!string.IsNullOrWhiteSpace(entityId))
        {
            query = query.Where(al => al.EntityId == entityId);
        }

        return await query
            .OrderByDescending(al => al.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _dbContext.Set<AuditLog>().AddAsync(auditLog);
    }
}
