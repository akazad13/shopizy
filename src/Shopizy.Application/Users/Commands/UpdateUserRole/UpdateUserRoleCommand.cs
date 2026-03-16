using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;

namespace Shopizy.Application.Users.Commands.UpdateUserRole;

public record UpdateUserRoleCommand(Guid UserId, string Role, List<Guid> PermissionIds) : ICommand<ErrorOr<Success>>;
