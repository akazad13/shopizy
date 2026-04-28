using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

public record DeleteProductImageCommand(Guid UserId, Guid ProductId, Guid ImageId)
    : ICommand<ErrorOr<Success>>;
