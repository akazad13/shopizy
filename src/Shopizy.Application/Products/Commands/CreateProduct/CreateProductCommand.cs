using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;

namespace Shopizy.Application.Products.Commands.CreateProduct;

[Authorize(Permissions = Permissions.Product.Create, Policies = Policy.SelfOrAdmin)]
public record CreateProductCommand(
    Guid UserId,
    string Name,
    string Description,
    Guid CategoryId,
    decimal UnitPrice,
    Currency Currency,
    decimal Discount,
    string Sku,
    string Brand,
    string Tags,
    string Barcode,
    IList<Guid>? SpecificationIds
) : IAuthorizeableRequest<ErrorOr<Product>>;
