using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Commands.DeleteUserAddress;

public record DeleteUserAddressCommand(Guid UserId, Guid AddressId) : ICommand<ErrorOr<Deleted>>;
