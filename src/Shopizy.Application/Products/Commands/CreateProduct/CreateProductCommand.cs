using ErrorOr;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;
using Microsoft.AspNetCore.Http;

namespace Shopizy.Application.Products.Commands.CreateProduct;

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
    List<IFormFile>? Images,
    List<Guid>? SpecificationIds
) : IAuthorizeableRequest<ErrorOr<Product>>;
