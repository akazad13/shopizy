using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Users.Commands.UpdateUserRole;

public class UpdateUserRoleCommandHandler(IUserRepository userRepository)
    : ICommandHandler<UpdateUserRoleCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<Success>> Handle(UpdateUserRoleCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(UserId.Create(command.UserId));
        if (user == null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.UpdatePermissions(command.PermissionIds.Select(PermissionId.Create).ToList());
        _userRepository.Update(user);

        return Result.Success;
    }
}
