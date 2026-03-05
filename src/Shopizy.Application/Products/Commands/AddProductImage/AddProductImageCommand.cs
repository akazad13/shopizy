using Shopizy.SharedKernel.Application.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public record AddProductImageCommand(Guid UserId, Guid ProductId, IFormFile? File)
    : ICommand<ErrorOr<ProductImage>>;

