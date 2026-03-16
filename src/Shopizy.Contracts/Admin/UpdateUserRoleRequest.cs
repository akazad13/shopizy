namespace Shopizy.Contracts.Admin;

public record UpdateUserRoleRequest(string Role, List<Guid> PermissionIds);
