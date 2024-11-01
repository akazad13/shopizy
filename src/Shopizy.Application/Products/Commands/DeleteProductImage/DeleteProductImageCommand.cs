using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

[Authorize(Permissions = Permissions.Product.Delete, Policies = Policy.SelfOrAdmin)]
public record DeleteProductImageCommand(Guid UserId, Guid ProductId, Guid ImageId)
    : IAuthorizeableRequest<IResult<GenericResponse>>;
