using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Returns.Commands.ApproveReturn;

public record ApproveReturnCommand(Guid ReturnRequestId) : ICommand<ErrorOr<Success>>;
