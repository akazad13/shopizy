using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.AuditLogs;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    : IQueryHandler<GetAuditLogsQuery, ErrorOr<IReadOnlyList<AuditLog>>>
{
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;

    public async Task<ErrorOr<IReadOnlyList<AuditLog>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken
    )
    {
        var logs = await _auditLogRepository.GetLogsAsync(
            request.EntityName,
            request.EntityId,
            request.PageNumber,
            request.PageSize
        );

        return ErrorOrFactory.From(logs);
    }
}
