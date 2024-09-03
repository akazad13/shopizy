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
        _ = config.NewConfig<AuthResult, AuthResponse>();
        _ = config.NewConfig<LoginRequest, LoginQuery>();
        _ = config.NewConfig<RegisterRequest, RegisterCommand>();
    }
}
