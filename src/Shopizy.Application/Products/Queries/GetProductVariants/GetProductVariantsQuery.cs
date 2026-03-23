using ErrorOr;
using Shopizy.Domain.Products.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Queries.GetProductVariants;

public record GetProductVariantsQuery(Guid ProductId) : IQuery<ErrorOr<IReadOnlyList<ProductVariant>>>;
