using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.DisableTwoFactor;

public record DisableTwoFactorCommand(Guid UserId) : ICommand<ErrorOr<Success>>;
