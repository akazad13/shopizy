using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Commands.CreateProduct;

[Authorize(Permissions = Permissions.Product.Create, Policies = Policy.Admin)]
public record CreateProductCommand(
    string Name,
    string ShortDescription,
    string Description,
    Guid CategoryId,
    decimal UnitPrice,
    Currency Currency,
    decimal Discount,
    string Sku,
    string Brand,
    string Colors,
    string Sizes,
    string Tags,
    string Barcode,
    IList<Guid>? SpecificationIds
) : IAuthorizeableRequest<ErrorOr<Product>>;
