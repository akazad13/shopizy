using ErrorOr;
using Shopizy.Domain.Returns;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Queries.GetReturnsByOrder;

public record GetReturnsByOrderQuery(Guid OrderId) : IQuery<ErrorOr<IReadOnlyList<ReturnRequest>>>;
