namespace Shopizy.Contracts.User;

public record UpdateUserAddressRequest(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
);
