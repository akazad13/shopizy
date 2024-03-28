using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

[Authorize(Permissions = Permission.Product.Delete, Policies = Policy.SelfOrAdmin)]
public record DeleteProductImageCommand(Guid UserId, Guid ProductId, Guid ImageId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
