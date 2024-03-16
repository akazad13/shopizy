using Mapster;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Contracts.Product;
using Shopizy.Domain.Products;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Categories.ValueObjects;

namespace Shopizy.Api.Common.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<(Guid UserId, CreateProductRequest request), CreateProductCommand>()
            .Map(dest => dest.UserId, src => UserId.Create(src.UserId))
            .Map(dest => dest.CategoryId, src => CategoryId.Create(src.request.CategoryId))
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<Product, ProductResponse>()
            .Map(dest => dest.ProductId, src => src.Id.Value)
            .Map(dest => dest.CategoryId, src => src.CategoryId.Value)
            .Map(dest => dest.Sku, src => src.SKU)
            .Map(dest => dest.Price, src => src.UnitPrice.Amount.ToString());
    }
}
