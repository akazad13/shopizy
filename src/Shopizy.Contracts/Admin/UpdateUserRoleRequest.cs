namespace Shopizy.Contracts.Admin;

/// <summary>
/// Admin-only request to change a user's role and replace their permission set.
/// </summary>
/// <param name="Role">Target role (e.g., "Customer", "Admin").</param>
/// <param name="PermissionIds">Replacement set of permission ids to assign.</param>
public record UpdateUserRoleRequest(string Role, IReadOnlyList<Guid> PermissionIds);
