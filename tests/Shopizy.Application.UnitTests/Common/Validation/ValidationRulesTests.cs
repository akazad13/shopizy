using FluentValidation;
using FluentValidation.TestHelper;
using Shopizy.Application.Common.Validation;
using Shouldly;

namespace Shopizy.Application.UnitTests.Common.Validation;

public class ValidationRulesTests
{
    private class DummyPasswordModel
    {
        public string Password { get; set; } = string.Empty;
    }

    private class DummyPasswordValidator : AbstractValidator<DummyPasswordModel>
    {
        public DummyPasswordValidator()
        {
            RuleFor(x => x.Password).StrongPassword();
        }
    }

    private class DummyPaginationModel
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

    private class DummyPaginationValidator : AbstractValidator<DummyPaginationModel>
    {
        public DummyPaginationValidator()
        {
            RuleFor(x => x.PageSize).ValidPageSize();
            RuleFor(x => x.PageNumber).ValidPageNumber();
        }
    }

    [Theory]
    [InlineData("Short1!")]
    [InlineData("alllowercase1!")]
    [InlineData("ALLUPPERCASE1!")]
    [InlineData("NoSpecialChar123")]
    [InlineData("NoDigitSpecialChar!")]
    public void StrongPassword_WhenInvalid_ShouldHaveValidationError(string password)
    {
        // Arrange
        var validator = new DummyPasswordValidator();
        var model = new DummyPasswordModel { Password = password };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void StrongPassword_WhenValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var validator = new DummyPasswordValidator();
        var model = new DummyPasswordModel { Password = "ValidP@ssw0rd123" };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void ValidPageSize_WhenInvalid_ShouldHaveValidationError(int pageSize)
    {
        // Arrange
        var validator = new DummyPaginationValidator();
        var model = new DummyPaginationModel { PageSize = pageSize };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void ValidPageSize_WhenValid_ShouldNotHaveValidationError(int pageSize)
    {
        // Arrange
        var validator = new DummyPaginationValidator();
        var model = new DummyPaginationModel { PageSize = pageSize };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidPageNumber_WhenInvalid_ShouldHaveValidationError(int pageNumber)
    {
        // Arrange
        var validator = new DummyPaginationValidator();
        var model = new DummyPaginationModel { PageNumber = pageNumber };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void ValidPageNumber_WhenValid_ShouldNotHaveValidationError(int pageNumber)
    {
        // Arrange
        var validator = new DummyPaginationValidator();
        var model = new DummyPaginationModel { PageNumber = pageNumber };

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }
}
