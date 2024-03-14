using ErrorOr;
using shopizy.Application.Common.Security.Request;
using shopizy.Application.Common.Security.Permissions;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;
using shopizy.Application.Common.Security.Policies;

namespace shopizy.Application.Products.Commands.CreateProduct;

[Authorize(Permissions = Permission.Product.Create, Policies = Policy.SelfOrAdmin)]
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
    int StockQuantity,
    List<Guid>? SpecificationIds
) : IAuthorizeableRequest<ErrorOr<Product>>;
