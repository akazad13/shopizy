using ErrorOr;
using Shopizy.Domain.Returns;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Queries.GetPendingReturns;

public record GetPendingReturnsQuery() : IQuery<ErrorOr<IReadOnlyList<ReturnRequest>>>;
