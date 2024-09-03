using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, ErrorOr<User>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));

        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        return user;
    }
}
