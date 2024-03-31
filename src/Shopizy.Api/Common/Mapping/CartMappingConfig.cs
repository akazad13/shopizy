using Mapster;
using Shopizy.Application.Carts.Commands.AddProductToCart;
using Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;
using Shopizy.Application.Carts.Commands.RemoveProductsFromCart;
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

        config
            .NewConfig<(Guid UserId, Guid CustomerId), GetCartQuery>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.CustomerId, src => src.CustomerId);

        config
            .NewConfig<Cart, CartResponse>()
            .Map(dest => dest.CartId, src => src.Id.Value)
            .Map(dest => dest.CustomerId, src => src.CustomerId.Value)
            .Map(dest => dest.LineItems, src => src.LineItems);

        config
            .NewConfig<LineItem, LineItemResponse>()
            .Map(dest => dest.LineItemId, src => src.Id.Value)
            .Map(dest => dest.ProductId, src => src.ProductId.Value)
            .Map(dest => dest.Name, src => src.Product.Name)
            .Map(dest => dest.Description, src => src.Product.Description)
            .Map(dest => dest.Price, src => src.Product.UnitPrice.Amount.ToString())
            .Map(dest => dest.Discount, src => src.Product.Name)
            .Map(dest => dest.Brand, src => src.Product.Name)
            .Map(dest => dest.StockQuantity, src => src.Product.StockQuantity)
            .Map(
                dest => dest.ProductImages,
                src => src.Product.ProductImages.Select(pi => pi.ImageUrl)
            );
    }
}
