using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
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
        Domain.Users.User? user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return CustomErrors.User.UserNotFound;
        }

        user.UpdateAddress(
            Address.CreateNew(
                street: request.Street,
                city: request.City,
                state: request.State,
                country: request.Country,
                zipCode: request.ZipCode
            )
        );

        _userRepository.Update(user);

        if (await _userRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.User.UserNotUpdated;
        }

        return Result.Success;
    }
}
