using ErrorOr;

namespace Shopizy.SharedKernel.Domain.Models;

/// <summary>
/// Domain-owned result type for aggregate methods that can fail.
/// Implicitly converts to <see cref="ErrorOr{TValue}"/> at the Application boundary.
/// </summary>
public readonly struct DomainResult<T>
{
    private readonly T? _value;
    private readonly DomainError? _error;

    private DomainResult(T value)
    {
        _value = value;
        _error = null;
    }

    private DomainResult(DomainError error)
    {
        _value = default;
        _error = error;
    }

    public bool IsError => _error.HasValue;
    public T Value => _value!;
    public DomainError Error => _error!.Value;

    public static implicit operator DomainResult<T>(T value) => new(value);

    public static implicit operator DomainResult<T>(DomainError error) => new(error);

    /// <summary>Converts to the Application-layer <see cref="ErrorOr{T}"/> type.</summary>
    public static implicit operator ErrorOr<T>(DomainResult<T> result) =>
        result.IsError ? result.Error.ToError() : result.Value!;
}
