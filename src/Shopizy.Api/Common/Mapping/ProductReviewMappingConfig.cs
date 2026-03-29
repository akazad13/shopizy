using Mapster;
using Shopizy.Application.ProductReviews.Commands.CreateProductReview;
using Shopizy.Contracts.ProductReview;
using Shopizy.Domain.ProductReviews;

namespace Shopizy.Api.Common.Mapping;

public class ProductReviewMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<(Guid UserId, Guid ProductId, CreateProductReviewRequest request), CreateProductReviewCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.Rating, src => src.request.Rating)
            .Map(dest => dest.Comment, src => src.request.Comment);

        config
            .NewConfig<ProductReview, ProductReviewResponse>()
            .Map(dest => dest.ReviewId, src => src.Id.Value)
            .Map(dest => dest.UserId, src => src.UserId.Value)
            .Map(dest => dest.UserName, src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : string.Empty)
            .Map(dest => dest.Rating, src => src.Rating.Value)
            .Map(dest => dest.Comment, src => src.Comment)
            .Map(dest => dest.CreatedOn, src => src.CreatedOn);
    }
}
