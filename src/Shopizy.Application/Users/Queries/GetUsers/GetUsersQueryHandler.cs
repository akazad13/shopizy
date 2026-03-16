using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUsersQuery, ErrorOr<IReadOnlyList<User>>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<IReadOnlyList<User>>> Handle(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListUsersAsync(query.PageNumber, query.PageSize);
        return users.ToErrorOr();
    }
}
