using Moq;
using Shopizy.Application.Brands.Commands.CreateBrand;
using Shopizy.Application.Brands.Commands.DeleteBrand;
using Shopizy.Application.Brands.Commands.UpdateBrand;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.Brands;

public class BrandCommandHandlerTests
{
    private readonly Mock<IBrandRepository> _mockBrandRepository;

    public BrandCommandHandlerTests()
    {
        _mockBrandRepository = new Mock<IBrandRepository>();
    }

    [Fact]
    public async Task CreateBrand_WhenDuplicateName_ShouldReturnDuplicateError()
    {
        var existing = Brand.Create("Nike", "logo.png", "USA");
        _mockBrandRepository.Setup(r => r.GetByNameAsync("Nike")).ReturnsAsync(existing);

        var handler = new CreateBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new CreateBrandCommand(Guid.NewGuid(), "Nike", "logo.png", "USA"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Brand.DuplicateName.Code);
    }

    [Fact]
    public async Task CreateBrand_WhenValid_ShouldAddBrand()
    {
        _mockBrandRepository.Setup(r => r.GetByNameAsync("Nike")).ReturnsAsync((Brand?)null);

        var handler = new CreateBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new CreateBrandCommand(Guid.NewGuid(), "Nike", "logo.png", "USA"),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        result.Value.Name.ShouldBe("Nike");
        _mockBrandRepository.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task DeleteBrand_WhenNotFound_ShouldReturnNotFoundError()
    {
        _mockBrandRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<BrandId>()))
            .ReturnsAsync((Brand?)null);

        var handler = new DeleteBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new DeleteBrandCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Brand.BrandNotFound.Code);
    }

    [Fact]
    public async Task DeleteBrand_WhenFound_ShouldRemoveBrand()
    {
        var brand = Brand.Create("Nike", "logo.png", "USA");
        _mockBrandRepository.Setup(r => r.GetByIdAsync(It.IsAny<BrandId>())).ReturnsAsync(brand);

        var handler = new DeleteBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new DeleteBrandCommand(Guid.NewGuid(), brand.Id.Value),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        _mockBrandRepository.Verify(r => r.Remove(brand), Times.Once);
    }

    [Fact]
    public async Task UpdateBrand_WhenNotFound_ShouldReturnNotFoundError()
    {
        _mockBrandRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<BrandId>()))
            .ReturnsAsync((Brand?)null);

        var handler = new UpdateBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new UpdateBrandCommand(Guid.NewGuid(), Guid.NewGuid(), "New Name", "logo.png", "USA"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Brand.BrandNotFound.Code);
    }

    [Fact]
    public async Task UpdateBrand_WhenDuplicateName_ShouldReturnDuplicateError()
    {
        var brand = Brand.Create("Nike", "logo.png", "USA");
        var otherBrand = Brand.Create("Adidas", "logo.png", "Germany");

        _mockBrandRepository
            .Setup(r => r.GetByIdAsync(It.Is<BrandId>(b => b.Value == brand.Id.Value)))
            .ReturnsAsync(brand);
        _mockBrandRepository.Setup(r => r.GetByNameAsync("Adidas")).ReturnsAsync(otherBrand);

        var handler = new UpdateBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new UpdateBrandCommand(Guid.NewGuid(), brand.Id.Value, "Adidas", "logo.png", "USA"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Brand.DuplicateName.Code);
    }

    [Fact]
    public async Task UpdateBrand_WhenValid_ShouldUpdate()
    {
        var brand = Brand.Create("Nike", "logo.png", "USA");
        _mockBrandRepository
            .Setup(r => r.GetByIdAsync(It.Is<BrandId>(b => b.Value == brand.Id.Value)))
            .ReturnsAsync(brand);
        _mockBrandRepository
            .Setup(r => r.GetByNameAsync("Nike Updated"))
            .ReturnsAsync((Brand?)null);

        var handler = new UpdateBrandCommandHandler(_mockBrandRepository.Object);
        var result = await handler.Handle(
            new UpdateBrandCommand(
                Guid.NewGuid(),
                brand.Id.Value,
                "Nike Updated",
                "new-logo.png",
                "USA"
            ),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        brand.Name.ShouldBe("Nike Updated");
    }
}
