using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.LoyaltyAccounts.Entities;
using Shopizy.Domain.LoyaltyAccounts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.LoyaltyAccounts.Persistence;

public sealed class LoyaltyAccountConfigurations : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        ConfigureLoyaltyAccountsTable(builder);
        ConfigureLoyaltyTransactionsTable(builder);
    }

    private static void ConfigureLoyaltyAccountsTable(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("LoyaltyAccounts");
        builder.HasKey(la => la.Id);

        builder
            .Property(la => la.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => LoyaltyAccountId.Create(value));

        builder
            .Property(la => la.UserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(la => la.TotalPoints).HasDefaultValue(0);
        builder.Property(la => la.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(la => la.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);
    }

    private static void ConfigureLoyaltyTransactionsTable(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.OwnsMany(
            la => la.Transactions,
            tb =>
            {
                tb.ToTable("LoyaltyTransactions");
                tb.WithOwner().HasForeignKey("LoyaltyAccountId");
                tb.HasKey(nameof(LoyaltyTransaction.Id), "LoyaltyAccountId");

                tb.Property(t => t.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => LoyaltyTransactionId.Create(value));

                tb.Property(t => t.Points);
                tb.Property(t => t.Type).HasConversion<string>();
                tb.Property(t => t.Description).HasMaxLength(200);
                tb.Property(t => t.CreatedOn).HasColumnType("smalldatetime");
            }
        );
        builder.Navigation(la => la.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
