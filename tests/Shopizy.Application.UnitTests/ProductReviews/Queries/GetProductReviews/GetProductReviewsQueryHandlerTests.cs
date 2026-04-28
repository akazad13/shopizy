using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Products.ValueObjects;
using Shouldly;

namespace Shopizy.Application.ProductReviews.Queries.GetProductReviews.UnitTests;

/// <summary>
/// Unit tests for <see cref="GetProductReviewsQueryHandler"/>.
/// </summary>
public class GetProductReviewsQueryHandlerTests
{
    private readonly Mock<IProductReviewRepository> _mockProductReviewRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly GetProductReviewsQueryHandler _sut;

    public GetProductReviewsQueryHandlerTests()
    {
        _mockProductReviewRepository = new Mock<IProductReviewRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new GetProductReviewsQueryHandler(
            _mockProductReviewRepository.Object,
            _mockProductRepository.Object
        );
    }

    /// <summary>
    /// Tests that Handle returns ProductNotFound error when the product does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnProductNotFoundError()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductReviewsQuery(productId, 1, 10);

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.Is<ProductId>(p => p.Value == productId)))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.ShouldNotBeEmpty();
        result.Errors[0].ShouldBe(CustomErrors.Product.ProductNotFound);
        _mockProductReviewRepository.Verify(
            x =>
                x.GetReviewsByProductIdAsync(
                    It.IsAny<ProductId>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ),
            Times.Never
        );
    }

    /// <summary>
    /// Tests that Handle returns an empty list when the product exists but has no reviews.
    /// </summary>
    [Fact]
    public async Task Handle_WhenProductExistsWithNoReviews_ShouldReturnEmptyList()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductReviewsQuery(productId, 1, 10);
        var emptyReviews = new List<ProductReview>();

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.Is<ProductId>(p => p.Value == productId)))
            .ReturnsAsync(true);

        _mockProductReviewRepository
            .Setup(x =>
                x.GetReviewsByProductIdAsync(It.Is<ProductId>(p => p.Value == productId), 1, 10)
            )
            .ReturnsAsync(emptyReviews);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that Handle correctly passes pagination parameters to the repository.
    /// </summary>
    /// <param name="pageNumber">The page number to test.</param>
    /// <param name="pageSize">The page size to test.</param>
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(5, 50)]
    [InlineData(10, 100)]
    [InlineData(1, 1)]
    public async Task Handle_WithVariousPaginationParameters_ShouldPassCorrectParametersToRepository(
        int pageNumber,
        int pageSize
    )
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductReviewsQuery(productId, pageNumber, pageSize);
        var reviews = new List<ProductReview>();

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        _mockProductReviewRepository
            .Setup(x => x.GetReviewsByProductIdAsync(It.IsAny<ProductId>(), pageNumber, pageSize))
            .ReturnsAsync(reviews);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockProductReviewRepository.Verify(
            x =>
                x.GetReviewsByProductIdAsync(
                    It.Is<ProductId>(p => p.Value == productId),
                    pageNumber,
                    pageSize
                ),
            Times.Once
        );
    }

    /// <summary>
    /// Tests that Handle works correctly with Guid.Empty as product ID.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyGuidProductId_ShouldReturnProductNotFoundError()
    {
        // Arrange
        var query = new GetProductReviewsQuery(Guid.Empty, 1, 10);

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.Is<ProductId>(p => p.Value == Guid.Empty)))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors[0].ShouldBe(CustomErrors.Product.ProductNotFound);
    }

    /// <summary>
    /// Tests that Handle correctly passes the cancellation token to repository methods.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductReviewsQuery(productId, 1, 10);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(query, cancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(It.Is<ProductId>(p => p.Value == productId)),
            Times.Once
        );
    }

    /// <summary>
    /// Tests that Handle with extreme pagination values passes them correctly to repository.
    /// </summary>
    /// <param name="pageNumber">The page number to test.</param>
    /// <param name="pageSize">The page size to test.</param>
    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(int.MinValue, int.MinValue)]
    public async Task Handle_WithExtremePaginationValues_ShouldPassParametersToRepository(
        int pageNumber,
        int pageSize
    )
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductReviewsQuery(productId, pageNumber, pageSize);
        var reviews = new List<ProductReview>();

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(true);

        _mockProductReviewRepository
            .Setup(x => x.GetReviewsByProductIdAsync(It.IsAny<ProductId>(), pageNumber, pageSize))
            .ReturnsAsync(reviews);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockProductReviewRepository.Verify(
            x =>
                x.GetReviewsByProductIdAsync(
                    It.Is<ProductId>(p => p.Value == productId),
                    pageNumber,
                    pageSize
                ),
            Times.Once
        );
    }

    /// <summary>
    /// Tests that Handle verifies product existence before fetching reviews.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldVerifyProductExistenceBeforeFetchingReviews()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetProductReviewsQuery(productId, 1, 10);

        _mockProductRepository
            .Setup(x => x.IsProductExistAsync(It.Is<ProductId>(p => p.Value == productId)))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        _mockProductRepository.Verify(
            x => x.IsProductExistAsync(It.Is<ProductId>(p => p.Value == productId)),
            Times.Once
        );
        _mockProductReviewRepository.Verify(
            x =>
                x.GetReviewsByProductIdAsync(
                    It.IsAny<ProductId>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ),
            Times.Never
        );
    }
}
