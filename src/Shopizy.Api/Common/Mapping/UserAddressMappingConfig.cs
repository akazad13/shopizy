using Mapster;
using Shopizy.Application.Users.Commands.AddUserAddress;
using Shopizy.Application.Users.Commands.UpdateUserAddress;
using Shopizy.Contracts.User;
using Shopizy.Domain.Users.Entities;

namespace Shopizy.Api.Common.Mapping;

public class UserAddressMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<(Guid UserId, AddUserAddressRequest request), AddUserAddressCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.Street, src => src.request.Street)
            .Map(dest => dest.City, src => src.request.City)
            .Map(dest => dest.State, src => src.request.State)
            .Map(dest => dest.Country, src => src.request.Country)
            .Map(dest => dest.ZipCode, src => src.request.ZipCode)
            .Map(dest => dest.IsDefault, src => src.request.IsDefault);

        config
            .NewConfig<(Guid UserId, Guid AddressId, UpdateUserAddressRequest request), UpdateUserAddressCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.AddressId, src => src.AddressId)
            .Map(dest => dest.Street, src => src.request.Street)
            .Map(dest => dest.City, src => src.request.City)
            .Map(dest => dest.State, src => src.request.State)
            .Map(dest => dest.Country, src => src.request.Country)
            .Map(dest => dest.ZipCode, src => src.request.ZipCode);

        config
            .NewConfig<UserAddress, UserAddressResponse>()
            .Map(dest => dest.AddressId, src => src.Id.Value)
            .Map(dest => dest.Street, src => src.Street)
            .Map(dest => dest.City, src => src.City)
            .Map(dest => dest.State, src => src.State)
            .Map(dest => dest.Country, src => src.Country)
            .Map(dest => dest.ZipCode, src => src.ZipCode)
            .Map(dest => dest.IsDefault, src => src.IsDefault)
            .Map(dest => dest.CreatedOn, src => src.CreatedOn);
    }
}
