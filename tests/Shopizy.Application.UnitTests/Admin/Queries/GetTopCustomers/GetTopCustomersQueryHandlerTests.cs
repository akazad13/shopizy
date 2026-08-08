using ErrorOr;
using FluentValidation.TestHelper;
using Moq;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Application.Admin.Queries.GetTopCustomers;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Admin.Queries.GetTopCustomers;

public class GetTopCustomersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetTopCustomersQueryHandler _handler;

    public GetTopCustomersQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _handler = new GetTopCustomersQueryHandler(_mockOrderRepository.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTopCustomersList()
    {
        // Arrange
        var query = new GetTopCustomersQuery(5);
        var expectedCustomers = new List<TopCustomerDto>
        {
            new(Guid.NewGuid(), "Jane", "Doe", 1500.00m),
        };

        _mockOrderRepository
            .Setup(r => r.GetTopCustomersBySpendAsync(5))
            .ReturnsAsync(expectedCustomers);

        // Act
        var result = await _handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].FirstName.ShouldBe("Jane");
        result.Value[0].LastName.ShouldBe("Doe");
        result.Value[0].TotalSpend.ShouldBe(1500.00m);
    }
}

public class GetTopCustomersQueryValidatorTests
{
    private readonly GetTopCustomersQueryValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Should_HaveError_When_CountIsOutOfRange(int count)
    {
        var query = new GetTopCustomersQuery(count);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(q => q.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Should_NotHaveError_When_CountIsValid(int count)
    {
        var query = new GetTopCustomersQuery(count);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
