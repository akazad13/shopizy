using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Events;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Domain.Products.Events;
using Shopizy.Domain.Wishlists;

namespace Shopizy.Application.UnitTests.Products.Events;

public class ProductBackInStockDomainEventHandlerTests
{
    private readonly Mock<IWishlistRepository> _mockWishlistRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<ProductBackInStockDomainEventHandler>> _mockLogger;
    private readonly ProductBackInStockDomainEventHandler _handler;

    public ProductBackInStockDomainEventHandlerTests()
    {
        _mockWishlistRepository = new Mock<IWishlistRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<ProductBackInStockDomainEventHandler>>();

        _handler = new ProductBackInStockDomainEventHandler(
            _mockWishlistRepository.Object,
            _mockUserRepository.Object,
            _mockEmailService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handle_WhenNoWishlistsContainProduct_ShouldNotSendEmails()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var domainEvent = new ProductBackInStockDomainEvent(product);

        _mockWishlistRepository
            .Setup(r => r.GetWishlistsByProductIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Wishlist>());

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockEmailService.Verify(
            e =>
                e.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task Handle_WhenWishlistsContainProduct_ShouldSendBackInStockEmail()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var domainEvent = new ProductBackInStockDomainEvent(product);

        var user = UserFactory.CreateUser();
        var wishlist = Wishlist.Create(user.Id);
        wishlist.AddItem(product.Id);

        _mockWishlistRepository
            .Setup(r => r.GetWishlistsByProductIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Wishlist> { wishlist });

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockEmailService.Verify(
            e =>
                e.SendAsync(
                    user.Email,
                    It.Is<string>(s => s.Contains("Back in Stock")),
                    It.Is<string>(b => b.Contains(product.Name)),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
