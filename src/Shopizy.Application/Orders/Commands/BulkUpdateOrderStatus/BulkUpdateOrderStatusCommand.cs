using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.BulkUpdateOrderStatus;

public record BulkUpdateOrderStatusCommand(IList<Guid> OrderIds, int Status) : ICommand<ErrorOr<Success>>;
