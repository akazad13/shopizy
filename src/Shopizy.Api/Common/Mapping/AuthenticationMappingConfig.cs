using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Auth.Commands.Register;
using Shopizy.Application.Auth.Common;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Contracts.Authentication;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for authentication-related models.
/// </summary>
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
