namespace Shopizy.Domain.Common.Models;

/// <summary>
/// Marker interface for entities that have creation and modification timestamps.
/// </summary>
public interface IAuditable
{
    DateTime CreatedOn { get; }
    DateTime? ModifiedOn { get; }
}
