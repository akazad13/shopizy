using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Carts.Commands.RemoveProductFromCart;
using Shopizy.Application.Carts.Commands.UpdateProductQuantity;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Contracts.Cart;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;

namespace Shopizy.Api.Common.Mapping;

public class CartMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config
            .NewConfig<
                (Guid UserId, CreateCartWithFirstProductRequest request),
                CreateCartWithFirstProductCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<
                (Guid UserId, Guid CartId, AddProductToCartRequest request),
                AddProductToCartCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CartId, src => src.CartId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<
                (Guid UserId, Guid CartId, Guid ItemId, UpdateProductQuantityRequest request),
                UpdateProductQuantityCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CartId, src => src.CartId)
            .Map(dest => dest.ItemId, src => src.ItemId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<(Guid UserId, Guid CartId, Guid ItemId), RemoveProductFromCartCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CartId, src => src.CartId)
            .Map(dest => dest.ItemId, src => src.ItemId);

        config.NewConfig<Guid, GetCartQuery>().MapWith(userId => new GetCartQuery(userId));

        config
            .NewConfig<Cart, CartResponse>()
            .Map(dest => dest.CartId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.CartItems, src => src.CartItems);

        config
            .NewConfig<CartItem, CartItemResponse>()
            .Map(dest => dest.CartItemId, src => src.Id.Value)
            .Map(dest => dest.ProductId, src => src.ProductId.Value)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Product, src => src.Product)
            .Map(
                dest => dest.Product.ProductImages,
                src =>
                    src.Product.ProductImages == null
                        ? null
                        : src.Product.ProductImages.Select(pi => pi.ImageUrl)
            )
            .Map(dest => dest.Product.Price, src => src.Product.UnitPrice.Amount);
    }
}
