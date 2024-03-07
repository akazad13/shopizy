using MediatR;

namespace shopizy.Application.Common.Security.Request;

public interface IAuthorizeableRequest<out T> : IRequest<T>
{
    Guid UserId { get; }
}
