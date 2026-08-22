using System.Security.Claims;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Payments.Queries.GetPaymentHistory;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Payment;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Payments;

public class GetPaymentHistoryEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                "api/v1.0/users/{userId:guid}/payments",
                async (
                    Guid userId,
                    ClaimsPrincipal user,
                    [FromServices] IDispatcher mediator,
                    IMapper mapper,
                    ILogger<GetPaymentHistoryEndpoint> logger
                ) =>
                {
                    if (user.AuthorizeOwner(userId, "payment history") is { } forbidden)
                        return forbidden;

                    var query = new GetPaymentHistoryQuery(userId);

                    return await HandleAsync(
                        mediator,
                        query,
                        payments => Results.Ok(mapper.Map<IReadOnlyList<PaymentDto>>(payments)),
                        ex => logger.PaymentFetchError(ex)
                    );
                }
            )
            .RequireAuthorization("Order.Read")
            .WithTags("Payments")
            .WithSummary("Get payment history for a user")
            .WithDescription("Retrieves the payment history associated with a specific user ID.")
            .Produces<IReadOnlyList<PaymentDto>>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
