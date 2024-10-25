using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

[Authorize(Permissions = Permissions.Product.Delete, Policies = Policy.SelfOrAdmin)]
public record DeleteProductCommand(Guid UserId, Guid ProductId)
    : IAuthorizeableRequest<IResult<GenericResponse>>;
