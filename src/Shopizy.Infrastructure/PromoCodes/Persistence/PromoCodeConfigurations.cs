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
        builder.ToTable("PromoCodes");
        builder.HasKey(pc => pc.Id);
        builder.HasIndex(pc => pc.Code);

        builder
            .Property(pc => pc.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PromoCodeId.Create(value));

        builder.Property(pc => pc.Code).HasMaxLength(15);
        builder.Property(pc => pc.Description).HasMaxLength(100).IsRequired(false);
        builder.Property(pc => pc.Discount).HasPrecision(18, 2);
        builder.Property(pc => pc.IsPerchantage).HasDefaultValue(true);
        builder.Property(pc => pc.IsActive).HasDefaultValue(true);
        builder.Property(pc => pc.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(pc => pc.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
        builder.Property(pc => pc.NumOfTimeUsed).HasDefaultValue(0);
    }
}
