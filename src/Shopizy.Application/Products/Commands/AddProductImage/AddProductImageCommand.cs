using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public record AddProductImageCommand(Guid UserId, Guid ProductId, IFormFile? File)
    : MediatR.IRequest<ErrorOr<ProductImage>>;
