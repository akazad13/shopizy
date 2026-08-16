using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.ProductReviews.Events;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.ProductReviews.Events;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.Application.UnitTests.ProductReviews.Events;

public class ProductReviewDeletedDomainEventHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ProductReviewDeletedDomainEventHandler _sut;

    public ProductReviewDeletedDomainEventHandlerTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _sut = new ProductReviewDeletedDomainEventHandler(
            _mockProductRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Should_DoNothing_WhenProductNotFound()
    {
        // Arrange
        var domainEvent = new ProductReviewDeletedDomainEvent(
            ProductId.CreateUnique(),
            Rating.CreateNew(5)
        );

        _mockProductRepo
            .Setup(x => x.GetProductByIdForUpdateAsync(It.IsAny<ProductId>()))
            .ReturnsAsync((Product?)null);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_RemoveReviewRatingAndSave_WhenProductFound()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var domainEvent = new ProductReviewDeletedDomainEvent(product.Id, Rating.CreateNew(5));

        _mockProductRepo
            .Setup(x => x.GetProductByIdForUpdateAsync(product.Id))
            .ReturnsAsync(product);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
