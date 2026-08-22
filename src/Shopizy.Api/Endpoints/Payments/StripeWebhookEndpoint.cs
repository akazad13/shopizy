using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Payments.Commands.ProcessStripeWebhook;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Payments;

public class StripeWebhookEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/webhooks/stripe",
                async (
                    HttpRequest httpRequest,
                    [FromServices] IDispatcher mediator,
                    ILogger<StripeWebhookEndpoint> logger,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (
                        !httpRequest.Headers.TryGetValue(
                            "Stripe-Signature",
                            out var signatureHeader
                        ) || string.IsNullOrWhiteSpace(signatureHeader)
                    )
                    {
                        return Results.BadRequest(
                            ErrorResult.Failure([
                                "stripe.missing_signature: Missing Stripe-Signature header.",
                            ])
                        );
                    }

                    using var reader = new StreamReader(httpRequest.Body);
                    var jsonPayload = await reader.ReadToEndAsync(cancellationToken);

                    var command = new ProcessStripeWebhookCommand(
                        jsonPayload,
                        signatureHeader.ToString()
                    );

                    return await HandleAsync(
                        mediator,
                        command,
                        _ =>
                            Results.Ok(
                                SuccessResult.Success(
                                    "Webhook received and processed successfully."
                                )
                            ),
                        ex => logger.StripeWebhookError(ex)
                    );
                }
            )
            .AllowAnonymous()
            .WithTags("Payments")
            .WithSummary("Stripe Webhook Handler")
            .WithDescription(
                "Receives and processes asynchronous payment events directly from Stripe."
            )
            .Produces<SuccessResult>(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
