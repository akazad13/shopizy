using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Users.Queries.GetUsers;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 10) : IQuery<ErrorOr<IReadOnlyList<User>>>;
