using ErrorOr;
using Shopizy.Domain.Users;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Users.Queries.GetUsers;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 10)
    : IQuery<ErrorOr<PagedResult<User>>>;
