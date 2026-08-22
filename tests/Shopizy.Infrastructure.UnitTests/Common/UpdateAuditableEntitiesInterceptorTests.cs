using Microsoft.EntityFrameworkCore;
using Moq;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Infrastructure.Common.Persistence.Interceptors;
using Shopizy.SharedKernel.Domain.Models;
using Shouldly;

namespace Shopizy.Infrastructure.UnitTests.Common;

public class UpdateAuditableEntitiesInterceptorTests
{
    private class TestAuditableEntity : IAuditable
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public TestAuditableEntity(Guid id)
        {
            Id = id;
        }
    }

    private class TestDbContext : DbContext
    {
        private readonly UpdateAuditableEntitiesInterceptor _interceptor;

        public DbSet<TestAuditableEntity> TestEntities => Set<TestAuditableEntity>();

        public TestDbContext(
            DbContextOptions<TestDbContext> options,
            UpdateAuditableEntitiesInterceptor interceptor
        )
            : base(options)
        {
            _interceptor = interceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_interceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestAuditableEntity>().HasKey(e => e.Id);
            base.OnModelCreating(modelBuilder);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetCreatedOnAndModifiedOn_ForAddedAndModifiedEntities()
    {
        // Arrange
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var fixedTime = new DateTime(2026, 8, 22, 12, 0, 0, DateTimeKind.Utc);
        mockDateTimeProvider.Setup(d => d.UtcNow).Returns(fixedTime);

        var interceptor = new UpdateAuditableEntitiesInterceptor(mockDateTimeProvider.Object);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new TestDbContext(options, interceptor);

        var entity = new TestAuditableEntity(Guid.NewGuid()) { Name = "Test" };
        context.TestEntities.Add(entity);

        // Act
        await context.SaveChangesAsync();

        // Assert
        entity.CreatedOn.ShouldBe(fixedTime);
        entity.ModifiedOn.ShouldBe(fixedTime);

        // Modify entity
        var updateTime = fixedTime.AddHours(1);
        mockDateTimeProvider.Setup(d => d.UtcNow).Returns(updateTime);

        entity.Name = "Updated Name";
        await context.SaveChangesAsync();

        // Assert update time
        entity.CreatedOn.ShouldBe(fixedTime);
        entity.ModifiedOn.ShouldBe(updateTime);
    }
}
