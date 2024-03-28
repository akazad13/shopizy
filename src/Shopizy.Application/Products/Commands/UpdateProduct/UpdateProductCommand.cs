using ErrorOr;
using Shopizy.Domain.Products;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Domain.Common.Enums;

namespace Shopizy.Application.Products.Commands.UpdateProduct;

[Authorize(Permissions = Permission.Product.Modify, Policies = Policy.SelfOrAdmin)]
public record UpdateProductCommand(
    Guid UserId,
    Guid ProductId,
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
    List<Guid>? SpecificationIds
) : IAuthorizeableRequest<ErrorOr<Product>>;
