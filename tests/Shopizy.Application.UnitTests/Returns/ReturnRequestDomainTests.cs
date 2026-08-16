using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Entities;
using Shopizy.Domain.Returns.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Returns;

public class ReturnRequestDomainTests
{
    private static ReturnRequest CreatePendingReturnRequest()
    {
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        return ReturnRequest.Create(
            OrderId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Item was defective",
            items
        );
    }

    [Fact]
    public void Create_ShouldCreateReturnRequestWithPendingStatus()
    {
        // Act
        var returnRequest = CreatePendingReturnRequest();

        // Assert
        returnRequest.Status.ShouldBe(ReturnStatus.Pending);
        returnRequest.Items.ShouldNotBeEmpty();
        returnRequest.AdminNote.ShouldBeNull();
    }

    [Fact]
    public void Approve_WhenPending_ShouldSetStatusToApproved()
    {
        // Arrange
        var returnRequest = CreatePendingReturnRequest();

        // Act
        var result = returnRequest.Approve();

        // Assert
        result.IsError.ShouldBeFalse();
        returnRequest.Status.ShouldBe(ReturnStatus.Approved);
    }

    [Fact]
    public void Approve_WhenAlreadyApproved_ShouldReturnError()
    {
        // Arrange
        var returnRequest = CreatePendingReturnRequest();
        returnRequest.Approve();

        // Act
        var result = returnRequest.Approve();

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public void Reject_WhenPending_ShouldSetStatusToRejectedWithNote()
    {
        // Arrange
        var returnRequest = CreatePendingReturnRequest();
        const string adminNote = "Not eligible for return after 30 days.";

        // Act
        var result = returnRequest.Reject(adminNote);

        // Assert
        result.IsError.ShouldBeFalse();
        returnRequest.Status.ShouldBe(ReturnStatus.Rejected);
        returnRequest.AdminNote.ShouldBe(adminNote);
    }

    [Fact]
    public void Reject_WhenAlreadyRejected_ShouldReturnError()
    {
        // Arrange
        var returnRequest = CreatePendingReturnRequest();
        returnRequest.Reject("First rejection");

        // Act
        var result = returnRequest.Reject("Second rejection");

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public void CompleteRefund_WhenApproved_ShouldSetStatusToRefunded()
    {
        // Arrange
        var returnRequest = CreatePendingReturnRequest();
        returnRequest.Approve();

        // Act
        var result = returnRequest.CompleteRefund();

        // Assert
        result.IsError.ShouldBeFalse();
        returnRequest.Status.ShouldBe(ReturnStatus.Refunded);
    }

    [Fact]
    public void CompleteRefund_WhenNotApproved_ShouldReturnError()
    {
        // Arrange
        var returnRequest = CreatePendingReturnRequest();

        // Act
        var result = returnRequest.CompleteRefund();

        // Assert
        result.IsError.ShouldBeTrue();
    }
}
