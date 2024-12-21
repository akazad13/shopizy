using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Policies;
using Shopizy.Application.Common.Security.Request;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.Products.Commands.AddProductImage;

[Authorize(Permissions = Permissions.Product.Create, Policies = Policy.Admin)]
public record AddProductImageCommand(Guid ProductId, IFormFile? File)
    : IAuthorizeableRequest<ErrorOr<ProductImage>>;
