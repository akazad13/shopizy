using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Products.Queries.ListProducts;
using shopizy.Contracts.Product;
using Shopizy.Contracts.Product;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Api.Common.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config.NewConfig<ProductsCriteriaRequest, ListProductsQuery>();

        config
            .NewConfig<(Guid UserId, CreateProductRequest request), CreateProductCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<
                (Guid UserId, Guid productId, UpdateProductRequest request),
                UpdateProductCommand
            >()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ProductId, src => src.productId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<(Guid UserId, Guid ProductId), DeleteProductCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ProductId, src => src.ProductId);

        config.NewConfig<Guid, GetProductQuery>().MapWith(src => new GetProductQuery(src));

        config
            .NewConfig<Product, ProductResponse>()
            .Map(dest => dest.ProductId, src => src.Id.Value)
            .Map(dest => dest.CategoryId, src => src.CategoryId.Value)
            .Map(dest => dest.Sku, src => src.SKU)
            .Map(dest => dest.Price, src => src.UnitPrice.Amount.ToString());

        config
            .NewConfig<ProductImage, ProductImageResponse>()
            .Map(dest => dest.ProductImageId, src => src.Id.Value);

        config
            .NewConfig<(Guid UserId, Guid ProductId, Guid ImageId), DeleteProductImageCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.ImageId, src => src.ImageId);
    }
}
