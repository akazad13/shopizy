namespace Shopizy.Contracts.Admin;

public record UpdateUserRoleRequest(string Role, IReadOnlyList<Guid> PermissionIds);
