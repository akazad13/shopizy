namespace Shopizy.Contracts.User;

public record UserAddressResponse(
    Guid AddressId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    bool IsDefault,
    DateTime CreatedOn
);
