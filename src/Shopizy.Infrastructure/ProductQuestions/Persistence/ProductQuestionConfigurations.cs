using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shopizy.Domain.ProductQuestions;
using Shopizy.Domain.ProductQuestions.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.ProductQuestions.Persistence;

public sealed class ProductQuestionConfigurations : IEntityTypeConfiguration<ProductQuestion>
{
    public void Configure(EntityTypeBuilder<ProductQuestion> builder)
    {
        ConfigureProductQuestionsTable(builder);
    }

    private static void ConfigureProductQuestionsTable(EntityTypeBuilder<ProductQuestion> builder)
    {
        builder.ToTable("ProductQuestions");
        builder.HasKey(pq => pq.Id);

        builder
            .Property(pq => pq.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => ProductQuestionId.Create(value));

        builder
            .Property(pq => pq.ProductId)
            .HasConversion(id => id.Value, value => ProductId.Create(value));

        builder
            .Property(pq => pq.AskedByUserId)
            .HasConversion(id => id.Value, value => UserId.Create(value));

        builder.Property(pq => pq.Question).HasMaxLength(500).IsRequired();
        builder.Property(pq => pq.IsAnswered).HasDefaultValue(false);
        builder.Property(pq => pq.CreatedOn).HasColumnType("smalldatetime");
        builder.Property(pq => pq.ModifiedOn).HasColumnType("smalldatetime").IsRequired(false);

        builder.OwnsOne(
            pq => pq.Answer,
            ab =>
            {
                ab.ToTable("ProductAnswers");

                ab.Property(a => a.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => ProductAnswerId.Create(value));

                ab
                    .Property(a => a.AnsweredByUserId)
                    .HasConversion(id => id.Value, value => UserId.Create(value));

                ab.Property(a => a.Answer).HasMaxLength(1000).IsRequired();
                ab.Property(a => a.CreatedOn).HasColumnType("smalldatetime");
            }
        );
    }
}
