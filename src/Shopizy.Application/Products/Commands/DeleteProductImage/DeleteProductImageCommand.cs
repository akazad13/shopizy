using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

public record DeleteProductImageCommand(Guid UserId, Guid ProductId, Guid ImageId)
    : ICommand<ErrorOr<Success>>;

