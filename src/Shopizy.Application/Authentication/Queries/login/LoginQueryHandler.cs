using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;

namespace Shopizy.Application.Authentication.Queries.login;

public class LoginQueryHandler(IJwtTokenGenerator _jwtTokenGenerator) : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        // validate if not null

        // check if the user present of not


        Guid userId = Guid.NewGuid();

        var firstName = "john";
        var lastName = "Doe";
        var roles = new List<string>();
        var permissions = new List<string>();

        var token = _jwtTokenGenerator.GenerateToken(userId, firstName, lastName, roles, permissions);

        return new AuthResult(
            Guid.NewGuid(),
            firstName,
            lastName,
            query.Phone,
            token
        );
    }
}