using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Products.Commands.DeleteProductImage;

[Authorize(Permissions = Permissions.Product.Delete, Policies = Policy.Admin)]
public record DeleteProductImageCommand(Guid UserId, Guid ProductId, Guid ImageId)
    : IAuthorizeableRequest<ErrorOr<Success>>;
