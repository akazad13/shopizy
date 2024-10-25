using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.User;
using Shopizy.Domain.Users;

namespace Shopizy.Api.Common.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config
            .NewConfig<(Guid UserId, UpdateAddressRequest request), UpdateAddressCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<(Guid UserId, UpdatePasswordRequest request), UpdatePasswordCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config.NewConfig<Guid, GetUserQuery>().MapWith(userId => new GetUserQuery(userId));
        config.NewConfig<User, UserDetails>();
    }
}
