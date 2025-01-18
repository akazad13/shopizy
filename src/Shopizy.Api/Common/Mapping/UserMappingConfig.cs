using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Commands.UpdateUser;
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
            .NewConfig<(Guid UserId, UpdateUserRequest request), UpdateUserCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request)
            .Map(dest => dest.Street, src => src.request.Address.Street)
            .Map(dest => dest.City, src => src.request.Address.City)
            .Map(dest => dest.State, src => src.request.Address.State)
            .Map(dest => dest.Country, src => src.request.Address.Country)
            .Map(dest => dest.ZipCode, src => src.request.Address.ZipCode);

        config
            .NewConfig<(Guid UserId, UpdateAddressRequest request), UpdateAddressCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<(Guid UserId, UpdatePasswordRequest request), UpdatePasswordCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config.NewConfig<Guid, GetUserQuery>().MapWith(userId => new GetUserQuery(userId));
        config
            .NewConfig<User, UserDetails>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.TotalOrders, src => src.OrderIds.Count)
            .Map(dest => dest.TotalProductsReviewed, src => src.ProductReviewIds.Count);
    }
}
