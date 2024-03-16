using MediatR;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Security.Request;

public interface IAuthorizeableRequest<out T> : IRequest<T>
{
    UserId UserId { get; }
}
