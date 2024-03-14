using Mapster;
using shopizy.Application.Products.Commands.CreateProduct;
using shopizy.Contracts.Product;
using Shopizy.Domain.Products;

namespace shopizy.Api.Common.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<(Guid UserId, CreateProductRequest request), CreateProductCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest, src => src.request);

        config
            .NewConfig<Product, ProductResponse>()
            .Map(dest => dest.ProductId, src => src.Id.Value)
            .Map(dest => dest.CategoryId, src => src.CategoryId.Value)
            .Map(dest => dest.Sku, src => src.SKU)
            .Map(dest => dest.Price, src => src.UnitPrice.Amount.ToString());
    }
}
