using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Carts.Persistence;
using Shopizy.Infrastructure.Common.Persistence;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Carts.Persistence;

public class CartRepositoryTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCart()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        var userId = UserId.CreateUnique();
        var cart = Cart.Create(userId);

        // Act
        await repository.AddAsync(cart);
        await dbContext.SaveChangesAsync();

        // Assert
        var result = await dbContext.Carts.FirstOrDefaultAsync(c => c.Id == cart.Id);
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(userId);
    }

    [Fact]
    public async Task GetCartByIdAsync_ShouldReturnCart()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        var userId = UserId.CreateUnique();
        var cart = Cart.Create(userId);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetCartByIdAsync(cart.Id, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(cart.Id);
    }

    [Fact]
    public async Task GetCartsAsync_ShouldReturnAllCarts()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        dbContext.Carts.Add(Cart.Create(UserId.CreateUnique()));
        dbContext.Carts.Add(Cart.Create(UserId.CreateUnique()));
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetCartsAsync();

        // Assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetCartByUserIdAsync_ShouldReturnCartWithItemsAndProducts()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        var userId = UserId.CreateUnique();
        var cart = Cart.Create(userId);

        var category = Category.Create("Electronics", null);
        var product = Product.Create(
            "Phone",
            "desc",
            "desc",
            category.Id,
            "SKU1",
            10,
            Price.CreateNew(100m, Currency.usd),
            null,
            null,
            "barcode",
            "color",
            "size",
            "tags"
        );

        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);

        var cartItem = CartItem.Create(product.Id, "color", "size", 2);
        cart.AddLineItem(cartItem);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        // Manually link the product for in-memory DB relation when not using full relational mapping
        cartItem.GetType().GetProperty("Product")?.SetValue(cartItem, product);

        // Act
        var result = await repository.GetCartByUserIdAsync(userId);

        // Assert
        result.ShouldNotBeNull();
        result.CartItems.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetCartByUserIdForUpdateAsync_ShouldReturnCartWithItems()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        var userId = UserId.CreateUnique();
        var cart = Cart.Create(userId);
        var productId = ProductId.CreateUnique();
        var cartItem = CartItem.Create(productId, "color", "size", 1);
        cart.AddLineItem(cartItem);
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetCartByUserIdForUpdateAsync(userId);

        // Assert
        result.ShouldNotBeNull();
        result.CartItems.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetAbandonedCartsAsync_ShouldReturnInactiveCarts()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);

        var cart1 = Cart.Create(UserId.CreateUnique());
        var cartItem = CartItem.Create(ProductId.CreateUnique(), "color", "size", 1);
        cart1.AddLineItem(cartItem);

        dbContext.Carts.Add(cart1);
        await dbContext.SaveChangesAsync();

        var entry1 = dbContext.Entry(cart1);
        entry1.Property("CreatedOn").CurrentValue = DateTime.UtcNow.AddDays(-2);
        entry1.Property("ModifiedOn").CurrentValue = DateTime.UtcNow.AddDays(-2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetAbandonedCartsAsync(DateTime.UtcNow.AddDays(-1));

        // Assert
        result.Count.ShouldBe(1);
        result[0].Id.ShouldBe(cart1.Id);
    }

    [Fact]
    public async Task Update_ShouldUpdateCart()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        var cart = Cart.Create(UserId.CreateUnique());
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        // Act
        var cartItem = CartItem.Create(ProductId.CreateUnique(), "color", "size", 1);
        cart.AddLineItem(cartItem);
        repository.Update(cart);
        await dbContext.SaveChangesAsync();

        // Assert
        var updated = await dbContext.Carts.FirstOrDefaultAsync(c => c.Id == cart.Id);
        updated.ShouldNotBeNull();
        updated.CartItems.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Remove_ShouldDeleteCart()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var repository = new CartRepository(dbContext);
        var cart = Cart.Create(UserId.CreateUnique());
        dbContext.Carts.Add(cart);
        await dbContext.SaveChangesAsync();

        // Act
        repository.Remove(cart);
        await dbContext.SaveChangesAsync();

        // Assert
        var deleted = await dbContext.Carts.FirstOrDefaultAsync(c => c.Id == cart.Id);
        deleted.ShouldBeNull();
    }
}
