using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.Products.Commands.AddProductImage;

[Authorize(Permissions = Permission.Product.Create, Policies = Policy.SelfOrAdmin)]
public record AddProductImageCommand(Guid UserId, Guid ProductId, IFormFile File)
    : IAuthorizeableRequest<ErrorOr<ProductImage>>;
