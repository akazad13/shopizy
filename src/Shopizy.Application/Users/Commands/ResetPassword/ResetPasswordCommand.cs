using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : ICommand<ErrorOr<Success>>;
