namespace Shopizy.Contracts.User;

/// <summary>
/// Represents a request to update a user's address.
/// </summary>
public class UpdateAddressRequest
{
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? ZipCode { get; set; }
}
