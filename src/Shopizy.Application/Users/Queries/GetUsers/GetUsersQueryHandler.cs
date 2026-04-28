using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUsersQuery, ErrorOr<PagedResult<User>>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<PagedResult<User>>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var users = await _userRepository.ListUsersAsync(query.PageNumber, query.PageSize);
        var totalCount = await _userRepository.GetTotalUsersCountAsync();
        return new PagedResult<User>(users, query.PageNumber, query.PageSize, totalCount);
    }
}
