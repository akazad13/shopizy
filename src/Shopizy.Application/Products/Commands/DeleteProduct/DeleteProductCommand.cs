using ErrorOr;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid UserId, Guid ProductId) : MediatR.IRequest<ErrorOr<Success>>;
