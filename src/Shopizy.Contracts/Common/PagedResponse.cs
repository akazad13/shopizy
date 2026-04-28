namespace Shopizy.Contracts.Common;

/// <summary>
/// A generic wrapper for paginated list responses.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public record PagedResponse<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    /// <summary>Total number of pages based on TotalCount and PageSize.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>Whether a previous page exists.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Whether a next page exists.</summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
