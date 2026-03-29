using ErrorOr;
using Shopizy.Domain.Users.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Users.Queries.GetUserAddresses;

public record GetUserAddressesQuery(Guid UserId) : IQuery<ErrorOr<List<UserAddress>>>;
