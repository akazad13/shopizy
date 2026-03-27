using FluentValidation;

namespace Shopizy.Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OldPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])")
            .WithMessage("New password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&).");
    }
}
