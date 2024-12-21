using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Common.Enums;

namespace Shopizy.Application.Products.Commands.UpdateProduct;

[Authorize(Permissions = Permissions.Product.Modify, Policies = Policy.Admin)]
public record UpdateProductCommand(
    Guid UserId,
    Guid ProductId,
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
) : IAuthorizeableRequest<ErrorOr<Success>>;
