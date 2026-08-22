using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.Categories.ValueObjects;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.Enums;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Infrastructure.PromoCodes.Persistence;

public sealed class PromoCodeConfigurations : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder) =>
        ConfigurePromoCodesTable(builder);

    private static void ConfigurePromoCodesTable(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable("PromoCodes");
        builder.HasKey(pc => pc.Id);
        builder.HasIndex(pc => pc.Code).IsUnique();

        builder
            .Property(pc => pc.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => PromoCodeId.Create(value));

        builder.Property(pc => pc.Code).HasMaxLength(15);
        builder.Property(pc => pc.Description).HasMaxLength(100).IsRequired(false);
        builder.Property(pc => pc.Discount).HasPrecision(18, 2);
        builder.Property(pc => pc.IsPercentage).HasDefaultValue(true);
        builder.Property(pc => pc.IsActive).HasDefaultValue(true);
        builder.Property(pc => pc.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(pc => pc.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
        builder.Property(pc => pc.NumOfTimeUsed).HasDefaultValue(0);

        builder.Property(pc => pc.PromoType).HasDefaultValue(PromoType.Standard);
        builder.Property(pc => pc.MinimumOrderAmount).HasPrecision(18, 2).IsRequired(false);
        builder.Property(pc => pc.MaxDiscountAmount).HasPrecision(18, 2).IsRequired(false);
#pragma warning disable CS8625
        builder
            .Property(pc => pc.TargetCategoryId)
            .HasConversion(
                id => (object?)id == null ? (Guid?)null : id.Value,
                value => value.HasValue ? CategoryId.Create(value.Value) : null
            )
            .IsRequired(false);
#pragma warning restore CS8625
        builder.Property(pc => pc.BuyQuantity).IsRequired(false);
        builder.Property(pc => pc.GetQuantity).IsRequired(false);
        builder.Property(pc => pc.GetDiscountPercentage).HasPrecision(18, 2).IsRequired(false);
        builder.Property(pc => pc.UsageLimit).IsRequired(false);
        builder.Property(pc => pc.StartDate).HasColumnType("smalldatetime").IsRequired(false);
        builder.Property(pc => pc.EndDate).HasColumnType("smalldatetime").IsRequired(false);
    }
}
