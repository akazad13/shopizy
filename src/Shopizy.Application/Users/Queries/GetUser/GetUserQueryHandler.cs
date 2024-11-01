using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Queries.GetUser;

public class GetUserQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserQuery, IResult<User>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IResult<User>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
        User? user = await _userRepository.GetUserById(UserId.Create(request.UserId));

        if (user is null)
        {
            return Response<User>.ErrorResponse([CustomErrors.User.UserNotFound]);
        }

        return Response<User>.SuccessResponese(user);
    }
}
