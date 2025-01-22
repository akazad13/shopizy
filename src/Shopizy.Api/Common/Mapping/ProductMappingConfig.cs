using Ardalis.GuardClauses;
using Mapster;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.Products.Commands.DeleteProduct;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.Products.Queries.GetProducts;
using shopizy.Contracts.Product;
using Shopizy.Contracts.Product;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Api.Common.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        Guard.Against.Null(config);

        config.NewConfig<ProductsCriteria, GetProductsQuery>();

        config.NewConfig<CreateProductRequest, CreateProductCommand>();

        config
            .NewConfig<(Guid productId, UpdateProductRequest request), UpdateProductCommand>()
            .Map(dest => dest.ProductId, src => src.productId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<Guid, DeleteProductCommand>()
            .MapWith(src => new DeleteProductCommand(src));

        config.NewConfig<Guid, GetProductQuery>().MapWith(src => new GetProductQuery(src));

        config
            .NewConfig<Product, ProductResponse>()
            .Map(dest => dest.ProductId, src => src.Id.Value)
            .Map(dest => dest.CategoryId, src => src.CategoryId.Value)
            .Map(dest => dest.Price, src => src.UnitPrice.Amount.ToString());

        config
            .NewConfig<Product, ProductDetailResponse>()
            .Map(dest => dest.ProductId, src => src.Id.Value)
            .Map(dest => dest.CategoryId, src => src.CategoryId.Value)
            .Map(dest => dest.Sku, src => src.SKU)
            .Map(dest => dest.Price, src => src.UnitPrice.Amount.ToString());

        config
            .NewConfig<ProductImage, ProductImageResponse>()
            .Map(dest => dest.ProductImageId, src => src.Id.Value);

        config
            .NewConfig<ProductReview, ProductReviewResponse>()
            .Map(dest => dest.ProductReviewId, src => src.Id.Value)
            .Map(dest => dest.Rating, src => src.Rating.Value)
            .Map(dest => dest.Reviewer, src => $"{src.User.FirstName} {src.User.LastName}")
            .Map(dest => dest.ReviewerImageUrl, src => src.User.ProfileImageUrl);

        config
            .NewConfig<(Guid ProductId, Guid ImageId), DeleteProductImageCommand>()
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.ImageId, src => src.ImageId);
    }
}
