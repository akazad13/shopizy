using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Commands.RequestReturn;

public record RequestReturnItemCommand(Guid OrderItemId, int Quantity);

public record RequestReturnCommand(
    Guid OrderId,
    Guid UserId,
    string Reason,
    IReadOnlyList<RequestReturnItemCommand> Items
) : ICommand<ErrorOr<Guid>>;
