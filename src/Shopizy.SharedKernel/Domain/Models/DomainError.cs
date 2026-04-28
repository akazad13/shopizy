using ErrorOr;

namespace Shopizy.SharedKernel.Domain.Models;

/// <summary>
/// Domain-owned error primitive. Use factory methods to construct; convert to
/// <see cref="ErrorOr.Error"/> at the Application boundary via <see cref="ToError"/>.
/// </summary>
public readonly struct DomainError
{
    public DomainErrorType ErrorType { get; }
    public string Code { get; }
    public string Description { get; }

    private DomainError(DomainErrorType errorType, string code, string description)
    {
        ErrorType = errorType;
        Code = code;
        Description = description;
    }

    public static DomainError NotFound(string code, string description) =>
        new(DomainErrorType.NotFound, code, description);

    public static DomainError Validation(string code, string description) =>
        new(DomainErrorType.Validation, code, description);

    public static DomainError Conflict(string code, string description) =>
        new(DomainErrorType.Conflict, code, description);

    public static DomainError Failure(string code, string description) =>
        new(DomainErrorType.Failure, code, description);

    public static DomainError Unauthorized(string code, string description) =>
        new(DomainErrorType.Unauthorized, code, description);

    /// <summary>Converts to the Application-layer <see cref="ErrorOr.Error"/> type.</summary>
    public Error ToError() =>
        ErrorType switch
        {
            DomainErrorType.NotFound => Error.NotFound(Code, Description),
            DomainErrorType.Validation => Error.Validation(Code, Description),
            DomainErrorType.Conflict => Error.Conflict(Code, Description),
            DomainErrorType.Unauthorized => Error.Unauthorized(Code, Description),
            _ => Error.Failure(Code, Description),
        };

    public static implicit operator Error(DomainError d) => d.ToError();
}

public enum DomainErrorType
{
    NotFound,
    Validation,
    Conflict,
    Failure,
    Unauthorized,
}
