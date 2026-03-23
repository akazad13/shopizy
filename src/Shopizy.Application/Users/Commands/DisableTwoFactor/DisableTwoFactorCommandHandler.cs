using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.DisableTwoFactor;

public class DisableTwoFactorCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DisableTwoFactorCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DisableTwoFactorCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userRepository.GetUserByIdAsync(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.DisableTwoFactor();
        userRepository.Update(user);

        return Result.Success;
    }
}
