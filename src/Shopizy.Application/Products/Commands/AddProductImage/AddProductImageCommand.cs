using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Domain.Products.Entities;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Commands.AddProductImage;

public record AddProductImageCommand(Guid UserId, Guid ProductId, IFormFile? File)
    : ICommand<ErrorOr<ProductImage>>;
