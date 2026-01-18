using ErrorOr;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

public record DeleteProductImageCommand(Guid UserId, Guid ProductId, Guid ImageId)
    : MediatR.IRequest<ErrorOr<Success>>;
