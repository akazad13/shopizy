using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid UserId, Guid ProductId) : ICommand<ErrorOr<Success>>;

