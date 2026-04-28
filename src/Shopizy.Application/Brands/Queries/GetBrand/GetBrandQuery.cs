using ErrorOr;
using Shopizy.Domain.Brands;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Queries.GetBrand;

public record GetBrandQuery(Guid BrandId) : IQuery<ErrorOr<Brand>>;
