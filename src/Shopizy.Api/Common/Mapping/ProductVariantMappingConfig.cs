using Mapster;
using Shopizy.Application.Products.Commands.AddVariant;
using Shopizy.Application.Products.Commands.UpdateVariant;
using Shopizy.Contracts.Product;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Configures mapping for product variant-related models.
/// </summary>
public class ProductVariantMappingConfig : IRegister
{
    /// <summary>
    /// Registers the mapping configurations.
    /// </summary>
    /// <param name="config">The type adapter configuration.</param>
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<ProductVariant, ProductVariantResponse>()
            .Map(dest => dest.VariantId, src => src.Id.Value)
            .Map(dest => dest.UnitPrice, src => src.UnitPrice.Amount)
            .Map(dest => dest.Currency, src => src.UnitPrice.Currency.ToString());

        config
            .NewConfig<(Guid ProductId, AddVariantRequest req), AddVariantCommand>()
            .MapWith(src => new AddVariantCommand(
                src.ProductId,
                src.req.Name,
                src.req.SKU,
                src.req.UnitPrice,
                Enum.Parse<Currency>(src.req.Currency, ignoreCase: true),
                src.req.StockQuantity
            ));

        config
            .NewConfig<(Guid ProductId, Guid VariantId, UpdateVariantRequest req), UpdateVariantCommand>()
            .MapWith(src => new UpdateVariantCommand(
                src.ProductId,
                src.VariantId,
                src.req.Name,
                src.req.SKU,
                src.req.UnitPrice,
                Enum.Parse<Currency>(src.req.Currency, ignoreCase: true),
                src.req.StockQuantity,
                src.req.IsActive
            ));
    }
}
