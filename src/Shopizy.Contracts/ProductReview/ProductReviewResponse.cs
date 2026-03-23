namespace Shopizy.Contracts.ProductReview;

public record ProductReviewResponse(
    Guid ReviewId,
    Guid UserId,
    string UserName,
    decimal Rating,
    string Comment,
    DateTime CreatedOn
);
