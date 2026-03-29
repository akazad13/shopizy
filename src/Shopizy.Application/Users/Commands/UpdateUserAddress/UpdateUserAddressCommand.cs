using ErrorOr;
using Shopizy.Domain.Users.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.UpdateUserAddress;

public record UpdateUserAddressCommand(
    Guid UserId,
    Guid AddressId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode
) : ICommand<ErrorOr<UserAddress>>;
