using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.GiftCards.Persistence;

public sealed class GiftCardConfigurations : IEntityTypeConfiguration<GiftCard>
{
    public void Configure(EntityTypeBuilder<GiftCard> builder)
    {
        ConfigureGiftCardsTable(builder);
    }

    private static void ConfigureGiftCardsTable(EntityTypeBuilder<GiftCard> builder)
    {
        builder.ToTable("GiftCards");
        builder.HasKey(gc => gc.Id);

        builder
            .Property(gc => gc.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => GiftCardId.Create(value));

        builder.Property(gc => gc.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(gc => gc.Code).IsUnique();

        builder.Property(gc => gc.InitialBalance).HasPrecision(18, 2);
        builder.Property(gc => gc.RemainingBalance).HasPrecision(18, 2);
        builder.Property(gc => gc.IsActive).HasDefaultValue(true);
        builder.Property(gc => gc.ExpiresOn).HasColumnType("smalldatetime").IsRequired(false);

        builder
            .Property(gc => gc.RedeemedByUserId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? UserId.Create(value.Value) : null
            )
            .IsRequired(false);

        builder.Property(gc => gc.RedeemedOn).HasColumnType("smalldatetime").IsRequired(false);
        builder.Property(gc => gc.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(gc => gc.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
    }
}
