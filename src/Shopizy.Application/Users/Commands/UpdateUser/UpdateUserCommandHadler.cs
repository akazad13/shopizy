using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<Success>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Street,
            request.City,
            request.State,
            request.Country,
            request.ZipCode
        );

        _userRepository.Update(user);

        if (await _userRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.User.UserNotUpdated;
        }

        return Result.Success;
    }
}
