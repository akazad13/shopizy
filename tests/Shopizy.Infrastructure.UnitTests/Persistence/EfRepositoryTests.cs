using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Entities;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Brands.Persistence;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.GiftCards.Persistence;
using Shopizy.Infrastructure.LoyaltyAccounts.Persistence;
using Shopizy.Infrastructure.Returns.Persistence;
using Shouldly;

namespace Shopizy.Infrastructure.UnitTests.Persistence;

public class EfRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var mockAccessor = new Mock<IHttpContextAccessor>();
        return new AppDbContext(options, mockAccessor.Object);
    }

    [Fact]
    public async Task BrandRepository_AddGetUpdateRemove_ShouldWorkCorrectly()
    {
        using var context = CreateDbContext();
        var repository = new BrandRepository(context);

        var brand = Brand.Create("Nike", "logo.png", "USA");
        await repository.AddAsync(brand);
        await context.SaveChangesAsync();

        var fetched = await repository.GetByIdAsync(brand.Id);
        fetched.ShouldNotBeNull();
        fetched.Name.ShouldBe("Nike");

        var byName = await repository.GetByNameAsync("Nike");
        byName.ShouldNotBeNull();

        var all = await repository.GetAsync();
        all.Count.ShouldBe(1);

        repository.Remove(brand);
        await context.SaveChangesAsync();

        var deleted = await repository.GetByIdAsync(brand.Id);
        deleted.ShouldBeNull();
    }

    [Fact]
    public async Task GiftCardRepository_AddGetUpdateRemove_ShouldWorkCorrectly()
    {
        using var context = CreateDbContext();
        var repository = new GiftCardRepository(context);

        var giftCard = GiftCard.Create("GIFT2026", 100m, DateTime.UtcNow.AddDays(30));
        await repository.AddAsync(giftCard);
        await context.SaveChangesAsync();

        var fetched = await repository.GetByIdAsync(giftCard.Id);
        fetched.ShouldNotBeNull();

        var byCode = await repository.GetByCodeAsync("GIFT2026");
        byCode.ShouldNotBeNull();

        var all = await repository.GetAllAsync(1, 10);
        all.Count.ShouldBe(1);

        repository.Remove(giftCard);
        await context.SaveChangesAsync();
        (await repository.GetByIdAsync(giftCard.Id)).ShouldBeNull();
    }

    [Fact]
    public async Task LoyaltyAccountRepository_AddGetUpdate_ShouldWorkCorrectly()
    {
        using var context = CreateDbContext();
        var repository = new LoyaltyAccountRepository(context);

        var userId = UserId.Create(Guid.NewGuid());
        var account = LoyaltyAccount.Create(userId);

        await repository.AddAsync(account);
        await context.SaveChangesAsync();

        var fetched = await repository.GetByUserIdAsync(userId);
        fetched.ShouldNotBeNull();
        fetched.UserId.ShouldBe(userId);

        account.EarnPoints(50, "Purchase");
        repository.Update(account);
        await context.SaveChangesAsync();

        var updated = await repository.GetByUserIdAsync(userId);
        updated!.TotalPoints.ShouldBe(50);
    }

    [Fact]
    public async Task ReturnRequestRepository_AddGetPendingByOrder_ShouldWorkCorrectly()
    {
        using var context = CreateDbContext();
        var repository = new ReturnRequestRepository(context);

        var orderId = OrderId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        var returnRequest = ReturnRequest.Create(orderId, userId, "Defective", items);

        await repository.AddAsync(returnRequest, CancellationToken.None);
        await context.SaveChangesAsync();

        var fetched = await repository.GetByIdAsync(returnRequest.Id, CancellationToken.None);
        fetched.ShouldNotBeNull();
        fetched.Reason.ShouldBe("Defective");

        var pending = await repository.GetPendingReturnsAsync(CancellationToken.None);
        pending.Count.ShouldBe(1);

        var byOrder = await repository.GetByOrderIdAsync(orderId, CancellationToken.None);
        byOrder.Count.ShouldBe(1);
    }
}
