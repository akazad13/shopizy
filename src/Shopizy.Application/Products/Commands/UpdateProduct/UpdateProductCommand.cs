using ErrorOr;
using Shopizy.Domain.Common.Enums;

namespace Shopizy.Application.Products.Commands.UpdateProduct;

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
) : MediatR.IRequest<ErrorOr<Success>>;
