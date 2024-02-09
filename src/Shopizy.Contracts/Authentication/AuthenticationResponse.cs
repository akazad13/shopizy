namespace Shopizy.Contracts.Authentication;

public record AuthResponse(Guid Id, string FirstName, string LastName, string Phone, string Token);
