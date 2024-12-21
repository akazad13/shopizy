using MediatR;

namespace Shopizy.Application.Common.Security.Request;

public interface IAuthorizeableRequest<out T> : IRequest<T> { }
