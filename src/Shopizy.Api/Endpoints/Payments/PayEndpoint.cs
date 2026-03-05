using System.Security.Claims;
using MapsterMapper;
using MediatR;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
using Shopizy.Application.Payments.Commands.CashOnDeliverySale;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Payment;

namespace Shopizy.Api.Endpoints.Payments;

public class PayEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/payments", async (Guid userId, CardNotPresentSaleRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<PayEndpoint> logger) =>
        {
            if (!user.IsAuthorized(userId))
            {
                return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to make a payment for this user.")]);
            }

            if (request.PaymentMethod.ToLower() == "card")
            {
                var command = mapper.Map<CardNotPresentSaleCommand>((userId, request));

                return await HandleAsync(
                    mediator,
                    command,
                    success => Results.Ok(SuccessResult.Success("Successfully processed payment.")),
                    ex => logger.PaymentError(ex)
                );
            }
            else
            {
                var command = mapper.Map<CashOnDeliverySaleCommand>((userId, request));

                return await HandleAsync(
                    mediator,
                    command,
                    success => Results.Ok(SuccessResult.Success("Successfully placed order.")),
                    ex => logger.PaymentError(ex)
                );
            }
        })
        .RequireAuthorization()
        .WithTags("Payments")
        .Produces<SuccessResult>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResult>(StatusCodes.Status409Conflict)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
