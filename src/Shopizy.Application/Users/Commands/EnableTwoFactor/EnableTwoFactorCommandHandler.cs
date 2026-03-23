using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.EnableTwoFactor;

public class EnableTwoFactorCommandHandler(IUserRepository userRepository)
    : ICommandHandler<EnableTwoFactorCommand, ErrorOr<TwoFactorSetupDto>>
{
    public async Task<ErrorOr<TwoFactorSetupDto>> Handle(
        EnableTwoFactorCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        var secret = user.EnableTwoFactor();
        userRepository.Update(user);

        var qrCodeUri = $"otpauth://totp/Shopizy:{user.Email}?secret={secret}&issuer=Shopizy";

        return new TwoFactorSetupDto(secret, qrCodeUri);
    }
}
