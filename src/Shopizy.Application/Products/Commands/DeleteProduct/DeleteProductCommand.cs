using ErrorOr;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;

namespace Shopizy.Application.Products.Commands.DeleteProduct;

[Authorize(Permissions = Permissions.Product.Delete, Policies = Policy.Admin)]
public record DeleteProductCommand(Guid UserId, Guid ProductId) : IAuthorizeableRequest<ErrorOr<Success>>;
