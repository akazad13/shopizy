using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.BulkDeleteProducts;

public record BulkDeleteProductsCommand(IList<Guid> ProductIds) : ICommand<ErrorOr<Deleted>>;
