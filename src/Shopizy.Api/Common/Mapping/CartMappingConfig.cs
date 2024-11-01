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
                (Guid UserId, Guid CartId, UpdateProductQuantityRequest request),
                UpdateProductQuantityCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CartId, src => src.CartId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<
                (Guid UserId, Guid CartId, RemoveProductFromCartRequest request),
                RemoveProductFromCartCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CartId, src => src.CartId)
            .Map(dest => dest, src => src.request);

        config.NewConfig<Guid, GetCartQuery>().MapWith(userId => new GetCartQuery(userId));

        config
            .NewConfig<Cart, CartResponse>()
            .Map(dest => dest.CartId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.LineItems, src => src.LineItems);

        config
            .NewConfig<LineItem, LineItemResponse>()
            .Map(dest => dest.LineItemId, src => src.Id.Value)
            .Map(dest => dest.ProductId, src => src.ProductId.Value)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Product, src => src.Product)
            .Map(
                dest => dest.Product.ProductImages,
                src =>
                    src.Product.ProductImages == null
                        ? null
                        : src.Product.ProductImages.Select(pi => pi.ImageUrl)
            );
    }
}
