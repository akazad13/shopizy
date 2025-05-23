using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Commands.UpdateAddress;

public class UpdateAddressCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateAddressCommand, ErrorOr<Success>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<ErrorOr<Success>> Handle(
        UpdateAddressCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.UpdateAddress(
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
