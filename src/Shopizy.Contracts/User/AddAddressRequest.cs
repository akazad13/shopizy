namespace Shopizy.Contracts.User;

public record AddAddressRequest(
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    bool IsDefault
);
