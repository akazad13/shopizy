using ErrorOr;
using Shopizy.Domain.Returns;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Returns.Queries.GetReturnById;

public record GetReturnByIdQuery(Guid ReturnId) : IQuery<ErrorOr<ReturnRequest>>;
