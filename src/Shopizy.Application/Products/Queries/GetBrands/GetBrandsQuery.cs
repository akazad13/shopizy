using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;

namespace Shopizy.Application.Products.Queries.GetBrands;

public record GetBrandsQuery() : IQuery<ErrorOr<IReadOnlyList<string>>>;
