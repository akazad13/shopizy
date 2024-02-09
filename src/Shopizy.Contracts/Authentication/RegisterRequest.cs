namespace Shopizy.Contracts.Authentication;

public record RegisterRequest(string FirstName, string LastName, string Phone, string Password);
