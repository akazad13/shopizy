using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using System.Security.Cryptography;

namespace Shopizy.Application.Users.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(IUserRepository userRepository)
    : ICommandHandler<ForgotPasswordCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email);

        // Security best practice: return success even if user not found
        if (user is null)
        {
            return string.Empty;
        }

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expiry = DateTime.UtcNow.AddHours(1);

        user.SetPasswordResetToken(token, expiry);
        userRepository.Update(user);

        // In production, send token via email; here we return it directly for dev purposes
        return token;
    }
}
