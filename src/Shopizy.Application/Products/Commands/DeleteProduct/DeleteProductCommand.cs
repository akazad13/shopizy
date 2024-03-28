using ErrorOr;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Permissions;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

[Authorize(Permissions = Permission.Product.Delete, Policies = Policy.SelfOrAdmin)]
public record DeleteProductCommand(Guid UserId, Guid ProductId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
