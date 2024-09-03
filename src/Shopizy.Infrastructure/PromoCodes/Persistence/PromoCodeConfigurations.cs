using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Infrastructure.PromoCodes.Persistence;

public sealed class PromoCodeConfigurations : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        ConfigurePromoCodesTable(builder);
    }

    private static void ConfigurePromoCodesTable(EntityTypeBuilder<PromoCode> builder)
    {
        _ = builder.ToTable("PromoCodes");
        _ = builder.HasKey(pc => pc.Id);
        _ = builder.HasIndex(pc => pc.Code);

        _ = builder
            .Property(pc => pc.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PromoCodeId.Create(value));

        _ = builder.Property(pc => pc.Code).HasMaxLength(15);
        _ = builder.Property(pc => pc.Description).HasMaxLength(100).IsRequired(false);
        _ = builder.Property(pc => pc.Discount).HasPrecision(18, 2);
        _ = builder.Property(pc => pc.IsPerchantage).HasDefaultValue(true);
        _ = builder.Property(pc => pc.IsActive).HasDefaultValue(true);
        _ = builder.Property(pc => pc.CreatedOn).HasColumnType("smalldatetime");
        _ = builder.Property(pc => pc.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
        _ = builder.Property(pc => pc.NumOfTimeUsed).HasDefaultValue(0);
    }
}
