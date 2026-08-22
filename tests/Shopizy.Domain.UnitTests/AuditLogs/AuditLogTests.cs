using Shopizy.Domain.AuditLogs;
using Shopizy.Domain.AuditLogs.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.AuditLogs;

public class AuditLogTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAuditLog()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var action = "Update";
        var entityName = "Product";
        var entityId = Guid.NewGuid().ToString();
        var oldValues = "{\"Name\":\"Old\"}";
        var newValues = "{\"Name\":\"New\"}";

        // Act
        var auditLog = AuditLog.Create(userId, action, entityName, entityId, oldValues, newValues);

        // Assert
        auditLog.ShouldNotBeNull();
        auditLog.Id.ShouldNotBeNull();
        auditLog.UserId.ShouldBe(userId);
        auditLog.Action.ShouldBe(action);
        auditLog.EntityName.ShouldBe(entityName);
        auditLog.EntityId.ShouldBe(entityId);
        auditLog.OldValues.ShouldBe(oldValues);
        auditLog.NewValues.ShouldBe(newValues);
        auditLog.Timestamp.ShouldBeInRange(
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddMinutes(1)
        );
    }

    [Fact]
    public void AuditLogId_CreateAndEquality_ShouldWorkAsExpected()
    {
        var guid = Guid.NewGuid();
        var id1 = AuditLogId.Create(guid);
        var id2 = AuditLogId.Create(guid);
        var unique = AuditLogId.CreateUnique();

        id1.Value.ShouldBe(guid);
        id1.ShouldBe(id2);
        id1.GetHashCode().ShouldBe(id2.GetHashCode());
        id1.ShouldNotBe(unique);
    }
}
