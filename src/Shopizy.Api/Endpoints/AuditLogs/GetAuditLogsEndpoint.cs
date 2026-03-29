using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.AuditLogs.Queries.GetAuditLogs;
using Shopizy.Contracts.AuditLog;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.AuditLogs;

public class GetAuditLogsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/admin/audit-logs",
            async (
                [FromQuery] string? entityName,
                [FromQuery] string? entityId,
                [FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetAuditLogsEndpoint> logger
            ) =>
            {
                return await HandleAsync(
                    mediator,
                    new GetAuditLogsQuery(entityName, entityId, pageNumber, pageSize),
                    logs => Results.Ok(mapper.Map<IReadOnlyList<AuditLogResponse>>(logs)),
                    ex => logger.AuditLogFetchError(ex)
                );
            }
        )
        .RequireAuthorization("Admin")
        .WithTags("AuditLogs")
        .WithSummary("Get audit logs")
        .WithDescription("Returns a paginated list of audit logs, optionally filtered by entity name and ID.")
        .Produces<IReadOnlyList<AuditLogResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
