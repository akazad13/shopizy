using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.Security.CurrentUserProvider;

[ExcludeFromCodeCoverage]
public record CurrentUser(Guid Id, IReadOnlyList<string> Permissions, IReadOnlyList<string> Roles);
