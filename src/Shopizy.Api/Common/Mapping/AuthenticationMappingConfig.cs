using Mapster;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Authentication.Queries.login;
using Shopizy.Contracts.Authentication;

namespace Shopizy.Api.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AuthResult, AuthResponse>();
        config.NewConfig<LoginRequest, LoginQuery>();
    }
}
