using Mapster;
using Shopizy.Application.Users.Commands.UpdatePassword;
using Shopizy.Application.Users.Commands.UpdateUser;
using Shopizy.Application.Users.Queries.GetUser;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.User;
using Shopizy.Domain.Users;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for user-related models.
/// </summary>
public class UserMappingConfig : IRegister
{
    /// <summary>
    /// Registers the mapping configurations.
    /// </summary>
    /// <param name="config">The type adapter configuration.</param>
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

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
            .NewConfig<(Guid UserId, UpdatePasswordRequest request), UpdatePasswordCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config.NewConfig<Guid, GetUserQuery>().MapWith(userId => new GetUserQuery(userId));

        config.NewConfig<UserDto, UserDetails>().Map(dest => dest.Id, src => src.Id.Value);

        config
            .NewConfig<User, UserDetails>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.Address, src => src.Address == null
                ? null
                : new Address(src.Address.Street, src.Address.City, src.Address.State, src.Address.Country, src.Address.ZipCode))
            .Map(dest => dest.TotalOrders, src => src.OrderIds.Count)
            .Map(dest => dest.TotalReviewed, src => src.ProductReviewIds.Count)
            .Map(dest => dest.TotalFavorites, src => 0)
            .Map(dest => dest.TotalReturns, src => 0);
    }
}
