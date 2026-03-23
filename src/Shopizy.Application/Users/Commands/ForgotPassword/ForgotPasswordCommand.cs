using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<ErrorOr<string>>;
