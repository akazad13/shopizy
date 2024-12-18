using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Authentication.Commands.Register;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Authentication.Queries.login;
using Shopizy.Contracts.Authentication;

namespace Shopizy.Api.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config.NewConfig<AuthResult, AuthResponse>();
        config.NewConfig<LoginRequest, LoginQuery>();
        config.NewConfig<RegisterRequest, RegisterCommand>();
    }
}
