using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.Extensions;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.Notifications.Commands.SendSms;
using Shopizy.Contracts.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.Notifications;

public record SendSmsRequest(string PhoneNumber, string Message);

public class SendSmsNotificationEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                "api/v1.0/notifications/sms",
                async (
                    [FromBody] SendSmsRequest request,
                    [FromServices] IDispatcher mediator,
                    ILogger<SendSmsNotificationEndpoint> logger
                ) =>
                {
                    var command = new SendSmsCommand(request.PhoneNumber, request.Message);

                    return await HandleAsync(
                        mediator,
                        command,
                        success => Results.Ok(new { Success = success }),
                        ex => logger.SmsDispatchError(ex)
                    );
                }
            )
            .RequireAuthorization("Admin")
            .WithTags("Notifications")
            .WithSummary("Send SMS notification")
            .WithDescription("Dispatches a transactional or alert SMS to a customer phone number.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ErrorResult>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResult>(StatusCodes.Status401Unauthorized)
            .Produces<ErrorResult>(StatusCodes.Status403Forbidden)
            .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
}
