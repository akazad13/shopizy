using FluentValidation.TestHelper;
using Moq;
using Shopizy.Application.Brands.Commands.CreateBrand;
using Shopizy.Application.Brands.Commands.DeleteBrand;
using Shopizy.Application.Brands.Commands.UpdateBrand;
using Shopizy.Application.Brands.Queries.GetBrand;
using Shopizy.Application.Brands.Queries.ListBrands;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shouldly;

namespace Shopizy.Application.UnitTests.Brands;

public class BrandQueriesAndValidatorsTests
{
    private readonly Mock<IBrandRepository> _mockRepository;

    public BrandQueriesAndValidatorsTests()
    {
        _mockRepository = new Mock<IBrandRepository>();
    }

    [Fact]
    public async Task GetBrandQueryHandler_WhenFound_ShouldReturnBrand()
    {
        // Arrange
        var brand = Brand.Create("Puma", "logo.png", "Germany");
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.Is<BrandId>(id => id.Value == brand.Id.Value)))
            .ReturnsAsync(brand);

        var handler = new GetBrandQueryHandler(_mockRepository.Object);
        var query = new GetBrandQuery(brand.Id.Value);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe("Puma");
    }

    [Fact]
    public async Task GetBrandQueryHandler_WhenNotFound_ShouldReturnBrandNotFoundError()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<BrandId>())).ReturnsAsync((Brand?)null);

        var handler = new GetBrandQueryHandler(_mockRepository.Object);
        var query = new GetBrandQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Brand.BrandNotFound.Code);
    }

    [Fact]
    public async Task ListBrandsQueryHandler_ShouldReturnBrandItemsList()
    {
        // Arrange
        var brands = new List<Brand>
        {
            Brand.Create("Adidas", "logo1.png", "Germany"),
            Brand.Create("Reebok", "logo2.png", "USA"),
        };
        _mockRepository.Setup(r => r.GetAsync()).ReturnsAsync(brands);

        var handler = new ListBrandsQueryHandler(_mockRepository.Object);
        var query = new ListBrandsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Name.ShouldBe("Adidas");
        result.Value[1].Name.ShouldBe("Reebok");
    }

    [Fact]
    public void CreateBrandValidator_ShouldValidateCorrectly()
    {
        var validator = new CreateBrandValidator();

        var validCommand = new CreateBrandCommand(Guid.NewGuid(), "Valid", "http://logo.png", "US");
        var validResult = validator.TestValidate(validCommand);
        validResult.ShouldNotHaveAnyValidationErrors();

        var invalidCommand = new CreateBrandCommand(Guid.NewGuid(), "", new string('x', 501), "");
        var invalidResult = validator.TestValidate(invalidCommand);
        invalidResult.ShouldHaveValidationErrorFor(c => c.Name);
        invalidResult.ShouldHaveValidationErrorFor(c => c.LogoUrl);
        invalidResult.ShouldHaveValidationErrorFor(c => c.Country);
    }

    [Fact]
    public void UpdateBrandValidator_ShouldValidateCorrectly()
    {
        var validator = new UpdateBrandValidator();

        var validCommand = new UpdateBrandCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Valid",
            "http://logo.png",
            "US"
        );
        var validResult = validator.TestValidate(validCommand);
        validResult.ShouldNotHaveAnyValidationErrors();

        var invalidCommand = new UpdateBrandCommand(
            Guid.NewGuid(),
            Guid.Empty,
            "",
            new string('x', 501),
            ""
        );
        var invalidResult = validator.TestValidate(invalidCommand);
        invalidResult.ShouldHaveValidationErrorFor(c => c.BrandId);
        invalidResult.ShouldHaveValidationErrorFor(c => c.Name);
        invalidResult.ShouldHaveValidationErrorFor(c => c.LogoUrl);
        invalidResult.ShouldHaveValidationErrorFor(c => c.Country);
    }

    [Fact]
    public void DeleteBrandValidator_ShouldValidateCorrectly()
    {
        var validator = new DeleteBrandValidator();

        var validCommand = new DeleteBrandCommand(Guid.NewGuid(), Guid.NewGuid());
        validator.TestValidate(validCommand).ShouldNotHaveAnyValidationErrors();

        var invalidCommand = new DeleteBrandCommand(Guid.NewGuid(), Guid.Empty);
        validator.TestValidate(invalidCommand).ShouldHaveValidationErrorFor(c => c.BrandId);
    }
}
