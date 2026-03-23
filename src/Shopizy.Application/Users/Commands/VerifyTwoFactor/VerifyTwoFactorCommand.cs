using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.VerifyTwoFactor;

public record VerifyTwoFactorCommand(Guid UserId, string Code) : ICommand<ErrorOr<Success>>;
