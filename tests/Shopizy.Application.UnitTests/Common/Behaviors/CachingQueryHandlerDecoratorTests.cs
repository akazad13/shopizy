using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.SharedKernel.Application.Behaviors;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.SharedKernel.Application.Messaging;
using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Behaviors;

public class CachingQueryHandlerDecoratorTests
{
    private readonly Mock<IQueryHandler<TestCachableQuery, TestCacheResponse>> _mockInnerHandler;
    private readonly Mock<ICacheHelper> _mockCacheHelper;
    private readonly Mock<ILogger<CachingQueryHandlerDecorator<TestCachableQuery, TestCacheResponse>>> _mockLogger;
    private readonly CachingQueryHandlerDecorator<TestCachableQuery, TestCacheResponse> _sut;

    public CachingQueryHandlerDecoratorTests()
    {
        _mockInnerHandler = new Mock<IQueryHandler<TestCachableQuery, TestCacheResponse>>();
        _mockCacheHelper = new Mock<ICacheHelper>();
        _mockLogger = new Mock<ILogger<CachingQueryHandlerDecorator<TestCachableQuery, TestCacheResponse>>>();
        _sut = new CachingQueryHandlerDecorator<TestCachableQuery, TestCacheResponse>(
            _mockInnerHandler.Object,
            _mockCacheHelper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ShouldReturnCachedValueWithoutCallingInnerHandler()
    {
        // Arrange
        var query = new TestCachableQuery();
        var cachedResponse = new TestCacheResponse { Value = "cached" };
        _mockCacheHelper
            .Setup(x => x.GetAsync<TestCacheResponse>(query.CacheKey))
            .ReturnsAsync(CacheResult<TestCacheResponse>.Hit(cachedResponse));

        // Act
        var result = await _sut.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(cachedResponse);
        result.Value.ShouldBe("cached");
        _mockInnerHandler.Verify(x => x.Handle(It.IsAny<TestCachableQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockCacheHelper.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TestCacheResponse>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldCallInnerHandlerAndCacheResult()
    {
        // Arrange
        var query = new TestCachableQuery();
        var handlerResponse = new TestCacheResponse { Value = "from-source" };
        _mockCacheHelper
            .Setup(x => x.GetAsync<TestCacheResponse>(query.CacheKey))
            .ReturnsAsync(CacheResult<TestCacheResponse>.Miss());
        _mockInnerHandler
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);
        _mockCacheHelper
            .Setup(x => x.SetAsync(query.CacheKey, handlerResponse, query.Expiration))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(handlerResponse);
        result.Value.ShouldBe("from-source");
        _mockInnerHandler.Verify(x => x.Handle(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheHelper.Verify(x => x.SetAsync(query.CacheKey, handlerResponse, query.Expiration), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenQueryIsNotCachable_ShouldPassThroughToInnerHandlerWithNoCacheInteraction()
    {
        // Arrange
        var nonCachableHandlerMock = new Mock<IQueryHandler<TestNonCachableQuery, TestCacheResponse>>();
        var sut = new CachingQueryHandlerDecorator<TestNonCachableQuery, TestCacheResponse>(
            nonCachableHandlerMock.Object,
            _mockCacheHelper.Object,
            new Mock<ILogger<CachingQueryHandlerDecorator<TestNonCachableQuery, TestCacheResponse>>>().Object);

        var query = new TestNonCachableQuery();
        var handlerResponse = new TestCacheResponse { Value = "direct" };
        nonCachableHandlerMock
            .Setup(x => x.Handle(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        // Act
        var result = await sut.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(handlerResponse);
        result.Value.ShouldBe("direct");
        nonCachableHandlerMock.Verify(x => x.Handle(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheHelper.Verify(x => x.GetAsync<TestCacheResponse>(It.IsAny<string>()), Times.Never);
        _mockCacheHelper.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TestCacheResponse>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    // Cachable query: implements both IQuery and ICachableRequest
    public class TestCachableQuery : IQuery<TestCacheResponse>, ICachableRequest
    {
        public string CacheKey => "test-cache-key";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }

    // Non-cachable query: implements only IQuery
    public class TestNonCachableQuery : IQuery<TestCacheResponse> { }

    public class TestCacheResponse
    {
        public string Value { get; set; } = string.Empty;
    }
}
