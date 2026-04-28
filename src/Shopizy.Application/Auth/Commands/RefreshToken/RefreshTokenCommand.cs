using ErrorOr;
using Shopizy.Application.Auth.Common;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<ErrorOr<AuthResult>>;
