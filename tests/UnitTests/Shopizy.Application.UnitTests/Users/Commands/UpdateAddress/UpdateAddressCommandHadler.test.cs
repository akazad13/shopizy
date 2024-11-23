using ErrorOr;
using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Commands.UpdateAddress;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Users.Commands.UpdateAddress;

public class UpdateAddressCommandHandlerTests
{
    private readonly UpdateAddressCommandHandler _sut;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public UpdateAddressCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _sut = new UpdateAddressCommandHandler(_mockUserRepository.Object);
    }

    // [Fact]
    // public async Task ShouldThrowExceptionWhenUserIdIsNotProvided()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = null, // Invalid user ID
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     // Act
    //     Func<Task<ErrorOr<Success>>> act = async () =>
    //         await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     await Assert.ThrowsAsync<ArgumentNullException>(act);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenUserIsNotFound()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync((Domain.Users.User)null);

    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<ErrorResult<Success>>();
    //     result.Errors.Should().Contain(CustomErrors.User.UserNotFound);
    // }

    [Fact]
    public async Task ShouldUpdateUserAddressSuccessfullyWhenUserIsFoundAsync()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        user = UserFactory.UpdateAddress(user);
        var command = UpdateAddressCommandUtils.CreateCommand();

        _mockUserRepository.Setup(u => u.GetUserById(It.IsAny<UserId>())).ReturnsAsync(user);

        _mockUserRepository.Setup(u => u.Update(It.IsAny<User>()));

        _mockUserRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<Success>();
        result.Value.Should().NotBeNull();

        _mockUserRepository.Verify(x => x.GetUserById(UserId.Create(command.UserId)), Times.Once);
        _mockUserRepository.Verify(x => x.Update(user), Times.Once);
        _mockUserRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task ShouldHandleConcurrentUpdatesToTheSameUser()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(
    //             new Domain.Users.User(
    //                 userId: UserId.Create("123"),
    //                 name: "John Doe",
    //                 email: "johndoe@example.com",
    //                 phoneNumber: "1234567890",
    //                 address: Address.CreateNew(
    //                     "123 Old St",
    //                     "Old City",
    //                     "Old State",
    //                     "Old Country",
    //                     "10000"
    //                 )
    //             )
    //         );

    //     var user1 = mockUserRepository.Object.GetUserById(UserId.Create("123")).Result;
    //     var user2 = mockUserRepository.Object.GetUserById(UserId.Create("123")).Result;

    //     // Act
    //     var task1 = handler.Handle(command, CancellationToken.None);
    //     var task2 = handler.Handle(command, CancellationToken.None);

    //     await Task.WhenAll(task1, task2);

    //     // Assert
    //     var result1 = task1.Result;
    //     var result2 = task2.Result;

    //     result1.Should().BeOfType<SuccessResult<Success>>();
    //     result2.Should().BeOfType<SuccessResult<Success>>();
    //     result1.Value.Message.Should().Be("Successfully updated address.");
    //     result2.Value.Message.Should().Be("Successfully updated address.");
    // }

    // [Fact]
    // public async Task ShouldValidateAddressFieldsForCorrectFormatAndLength()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St", // Valid street format
    //         City = "New York", // Valid city format
    //         State = "NY", // Valid state format
    //         Country = "USA", // Valid country format
    //         ZipCode = "10001", // Valid zip code format
    //     };

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(
    //             new Domain.Users.User(
    //                 userId: UserId.Create("123"),
    //                 name: "John Doe",
    //                 email: "johndoe@example.com",
    //                 phoneNumber: "1234567890",
    //                 address: Address.CreateNew(
    //                     "123 Old St",
    //                     "Old City",
    //                     "Old State",
    //                     "Old Country",
    //                     "10000"
    //                 )
    //             )
    //         );

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<SuccessResult<Success>>();
    //     result.Value.Message.Should().Be("Successfully updated address.");
    // }

    // [Fact]
    // public async Task ShouldHandleCasesWhereDatabaseConnectionIsLostDuringCommit()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(
    //             new Domain.Users.User(
    //                 userId: UserId.Create("123"),
    //                 name: "John Doe",
    //                 email: "johndoe@example.com",
    //                 phoneNumber: "1234567890",
    //                 address: Address.CreateNew(
    //                     "123 Old St",
    //                     "Old City",
    //                     "Old State",
    //                     "Old Country",
    //                     "10000"
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Update(It.IsAny<Domain.Users.User>()))
    //         .Callback<Domain.Users.User>(user =>
    //             user.UpdateAddress(
    //                 Address.CreateNew(
    //                     street: command.Street,
    //                     city: command.City,
    //                     state: command.State,
    //                     country: command.Country,
    //                     zipCode: command.ZipCode
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(0); // Simulate database connection loss

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<ErrorResult<Success>>();
    //     result.Errors.Should().Contain(CustomErrors.User.UserNotUpdated);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenDatabaseIsReadOnly()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var mockReadOnlyDatabase = new Mock<IDatabase>();
    //     mockReadOnlyDatabase.Setup(db => db.IsReadOnly).Returns(true);
    //     mockUserRepository.Setup(repo => repo.Database).Returns(mockReadOnlyDatabase.Object);

    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<ErrorResult<Success>>();
    //     result.Errors.Should().Contain(CustomErrors.Database.ReadOnly);
    // }

    // [Fact]
    // public async Task ShouldNotUpdateUserAddressWhenAddressIsUnchanged()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(
    //             new Domain.Users.User(
    //                 userId: UserId.Create("123"),
    //                 name: "John Doe",
    //                 email: "johndoe@example.com",
    //                 phoneNumber: "1234567890",
    //                 address: Address.CreateNew(
    //                     command.Street,
    //                     command.City,
    //                     command.State,
    //                     command.Country,
    //                     command.ZipCode
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Update(It.IsAny<Domain.Users.User>()))
    //         .Callback<Domain.Users.User>(user =>
    //             user.UpdateAddress(
    //                 Address.CreateNew(
    //                     street: command.Street,
    //                     city: command.City,
    //                     state: command.State,
    //                     country: command.Country,
    //                     zipCode: command.ZipCode
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(0); // No changes made, so no commit

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<SuccessResult<Success>>();
    //     result.Value.Message.Should().Be("No changes made to address.");
    // }

    // [Fact]
    // public async Task ShouldHandlePartialAddressChange()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "456 New St", // Changed street
    //         City = null, // Unchanged city
    //         State = "CA", // Changed state
    //         Country = null, // Unchanged country
    //         ZipCode = "90001", // Changed zip code
    //     };

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(
    //             new Domain.Users.User(
    //                 userId: UserId.Create("123"),
    //                 name: "John Doe",
    //                 email: "johndoe@example.com",
    //                 phoneNumber: "1234567890",
    //                 address: Address.CreateNew(
    //                     "123 Old St",
    //                     "Old City",
    //                     "Old State",
    //                     "Old Country",
    //                     "10000"
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Update(It.IsAny<Domain.Users.User>()))
    //         .Callback<Domain.Users.User>(user =>
    //             user.UpdateAddress(
    //                 Address.CreateNew(
    //                     street: command.Street ?? user.Address.Street,
    //                     city: command.City ?? user.Address.City,
    //                     state: command.State ?? user.Address.State,
    //                     country: command.Country ?? user.Address.Country,
    //                     zipCode: command.ZipCode ?? user.Address.ZipCode
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<SuccessResult<Success>>();
    //     result.Value.Message.Should().Be("Successfully updated address.");
    // }

    // [Fact]
    // public async Task ShouldHandleCasesWhereUserAddressIsUpdatedToExistingAddress()
    // {
    //     // Arrange
    //     var mockUserRepository = new Mock<IUserRepository>();
    //     var handler = new UpdateAddressCommandHandler(mockUserRepository.Object);
    //     var command = new UpdateAddressCommand
    //     {
    //         UserId = "123",
    //         Street = "123 Main St",
    //         City = "New York",
    //         State = "NY",
    //         Country = "USA",
    //         ZipCode = "10001",
    //     };

    //     mockUserRepository
    //         .Setup(repo => repo.GetUserById(It.IsAny<UserId>()))
    //         .ReturnsAsync(
    //             new Domain.Users.User(
    //                 userId: UserId.Create("123"),
    //                 name: "John Doe",
    //                 email: "johndoe@example.com",
    //                 phoneNumber: "1234567890",
    //                 address: Address.CreateNew("123 Main St", "New York", "NY", "USA", "10001")
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Update(It.IsAny<Domain.Users.User>()))
    //         .Callback<Domain.Users.User>(user =>
    //             user.UpdateAddress(
    //                 Address.CreateNew(
    //                     street: command.Street,
    //                     city: command.City,
    //                     state: command.State,
    //                     country: command.Country,
    //                     zipCode: command.ZipCode
    //                 )
    //             )
    //         );

    //     mockUserRepository
    //         .Setup(repo => repo.Commit(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(1);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.Should().BeOfType<SuccessResult<Success>>();
    //     result.Value.Message.Should().Be("Successfully updated address.");
    // }
}
