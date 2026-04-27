using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Auth.Common;

namespace Shopizy.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<ErrorOr<AuthResult>>;
