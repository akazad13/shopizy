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

public class PayEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v1.0/users/{userId:guid}/payments", async (Guid userId, CardNotPresentSaleRequest request, ClaimsPrincipal user, ISender mediator, IMapper mapper, ILogger<PayEndpoint> logger) =>
        {
            try
            {
                if (!user.IsAuthorized(userId))
                {
                    return CustomResults.Problem([ErrorOr.Error.Forbidden(description: "You are not authorized to make a payment for this user.")]);
                }

                if (request.PaymentMethod.ToLower() == "card")
                {
                    var command = mapper.Map<CardNotPresentSaleCommand>((userId, request));
                    var result = await mediator.Send(command);

                    return result.Match(
                        success => Results.Ok(SuccessResult.Success("Successfully processed payment.")),
                        CustomResults.Problem
                    );
                }
                else
                {
                    var command = mapper.Map<CashOnDeliverySaleCommand>((userId, request));
                    var result = await mediator.Send(command);

                    return result.Match(
                        success => Results.Ok(SuccessResult.Success("Successfully placed order.")),
                        CustomResults.Problem
                    );
                }
            }
            catch (Exception ex)
            {
                logger.PaymentError(ex);
                return CustomResults.Problem([ErrorOr.Error.Unexpected(description: ex.Message)]);
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
