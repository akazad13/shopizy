using MediatR;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordManager passwordManager
) : IRequestHandler<UpdatePasswordCommand, IResult<GenericResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<IResult<GenericResponse>> Handle(
        UpdatePasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        Domain.Users.User? user = await _userRepository.GetUserById(UserId.Create(request.UserId));
        if (user is null)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.User.UserNotFound]);
        }

        if (!_passwordManager.Verify(request.OldPassword, user.Password ?? ""))
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.User.PasswordNotCorrect]);
        }

        user.UpdatePassword(_passwordManager.CreateHashString(request.NewPassword));

        _userRepository.Update(user);

        if (await _userRepository.Commit(cancellationToken) <= 0)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.User.PasswordNotUpdated]);
        }

        return Response<GenericResponse>.SuccessResponese("Successfully updated password.");
    }
}
