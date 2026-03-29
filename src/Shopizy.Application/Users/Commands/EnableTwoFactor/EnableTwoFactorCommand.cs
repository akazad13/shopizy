using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.EnableTwoFactor;

public record EnableTwoFactorCommand(Guid UserId) : ICommand<ErrorOr<TwoFactorSetupDto>>;
