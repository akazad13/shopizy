using ErrorOr;
using Shopizy.Domain.AuditLogs;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.AuditLogs.Queries.GetAuditLogs;

public record GetAuditLogsQuery(
    string? EntityName,
    string? EntityId,
    int PageNumber,
    int PageSize
) : IQuery<ErrorOr<IReadOnlyList<AuditLog>>>;
