using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Notifications.Commands.SendSms;

public record SendSmsCommand(string PhoneNumber, string Message) : ICommand<ErrorOr<bool>>;
