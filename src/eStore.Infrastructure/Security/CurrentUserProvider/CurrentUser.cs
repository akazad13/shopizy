namespace eStore.Infrastructure.Security.CurrentUserProvider;

public record CurrentUser(
    Guid Id,
    string FirstName,
    string LastName,
    string Phone,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Roles
);
