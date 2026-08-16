using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Returns.Commands.RejectReturn;

public record RejectReturnCommand(Guid ReturnRequestId, string AdminNote)
    : ICommand<ErrorOr<Success>>;
