namespace Shopizy.Infrastructure.Security.CurrentUserProvider;

public record CurrentUser(Guid Id, IReadOnlyList<string> Permissions, IReadOnlyList<string> Roles);
