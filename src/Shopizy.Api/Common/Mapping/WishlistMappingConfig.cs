using Mapster;
using Shopizy.Application.Wishlists.Commands.CreateWishlist;
using Shopizy.Application.Wishlists.Commands.UpdateWishlist;
using Shopizy.Application.Wishlists.Queries.GetWishlist;
using Shopizy.Contracts.Wishlist;
using Shopizy.Domain.Wishlists;
using Shopizy.Domain.Wishlists.Entities;

namespace Shopizy.Api.Common.Mapping;

public class WishlistMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config.NewConfig<Guid, CreateWishlistCommand>()
            .MapWith(userId => new CreateWishlistCommand(userId));

        config.NewConfig<Guid, GetWishlistQuery>()
            .MapWith(userId => new GetWishlistQuery(userId));

        config
            .NewConfig<(Guid UserId, UpdateWishlistRequest request), UpdateWishlistCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ProductId, src => src.request.ProductId)
            .Map(
                dest => dest.Action,
                src =>
                    src.request.Action.Equals("Remove", StringComparison.OrdinalIgnoreCase)
                        ? WishlistAction.Remove
                        : WishlistAction.Add
            );

        config
            .NewConfig<Wishlist, WishlistResponse>()
            .Map(dest => dest.WishlistId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.WishlistItems, src => src.WishlistItems);

        config
            .NewConfig<WishlistItem, WishlistItemResponse>()
            .Map(dest => dest.WishlistItemId, src => src.Id.Value)
            .Map(dest => dest.ProductId, src => src.ProductId.Value);
    }
}
