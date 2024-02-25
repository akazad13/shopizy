using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using shopizy.Domain.PromoCodes;

namespace shopizy.Infrastructure.PromoCodes.Persistence;

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

        builder.Property(pc => pc.Code).HasMaxLength(15);
        builder.Property(pc => pc.Description).HasMaxLength(100);
        builder.Property(pc => pc.Discount);
        builder.Property(pc => pc.IsPerchantage).HasDefaultValue(true);
        builder.Property(pc => pc.IsActive).HasDefaultValue(true);
        builder.Property(pc => pc.CreatedOn);
        builder.Property(pc => pc.ModifiedOn);
        builder.Property(pc => pc.NumOfTimeUsed);
    }
}
