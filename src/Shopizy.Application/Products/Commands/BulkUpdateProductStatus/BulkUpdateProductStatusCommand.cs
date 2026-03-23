using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.BulkUpdateProductStatus;

public record BulkUpdateProductStatusCommand(IList<Guid> ProductIds, bool IsActive) : ICommand<ErrorOr<Success>>;
