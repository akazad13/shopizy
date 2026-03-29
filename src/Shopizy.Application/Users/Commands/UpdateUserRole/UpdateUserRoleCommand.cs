using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;

namespace Shopizy.Application.Users.Commands.UpdateUserRole;

public record UpdateUserRoleCommand(Guid UserId, string Role, IReadOnlyList<Guid> PermissionIds, Guid ModifiedById) : ICommand<ErrorOr<Success>>;
