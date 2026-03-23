using ErrorOr;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommandHandler(
    IUserRepository userRepository,
    ITotpHelper totpHelper
) : ICommandHandler<VerifyTwoFactorCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        VerifyTwoFactorCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        if (string.IsNullOrEmpty(user.TwoFactorSecret))
        {
            return Error.Validation("TwoFactor.NotSetup", "Two-factor authentication has not been set up.");
        }

        if (!totpHelper.VerifyCode(user.TwoFactorSecret, request.Code))
        {
            return Error.Validation("TwoFactor.InvalidCode", "Invalid code.");
        }

        user.ConfirmTwoFactor();
        userRepository.Update(user);

        return Result.Success;
    }
}
