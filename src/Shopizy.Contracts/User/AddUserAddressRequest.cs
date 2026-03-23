namespace Shopizy.Contracts.User;

public record AddUserAddressRequest(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    bool IsDefault = false
);
