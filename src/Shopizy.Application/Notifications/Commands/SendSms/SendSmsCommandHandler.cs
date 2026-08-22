using ErrorOr;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Notifications.Commands.SendSms;

public class SendSmsCommandHandler(ISmsService smsService)
    : ICommandHandler<SendSmsCommand, ErrorOr<bool>>
{
    private readonly ISmsService _smsService = smsService;

    public async Task<ErrorOr<bool>> Handle(
        SendSmsCommand request,
        CancellationToken cancellationToken
    )
    {
        var sent = await _smsService.SendSmsAsync(
            request.PhoneNumber,
            request.Message,
            cancellationToken
        );

        if (!sent)
        {
            return Error.Failure("Sms.FailedToSend", "Failed to dispatch SMS text message.");
        }

        return true;
    }
}
