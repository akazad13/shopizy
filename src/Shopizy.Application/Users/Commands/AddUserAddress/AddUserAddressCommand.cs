using ErrorOr;
using Shopizy.Domain.Users.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.AddUserAddress;

public record AddUserAddressCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    bool IsDefault
) : ICommand<ErrorOr<UserAddress>>;
