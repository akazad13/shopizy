using Ardalis.GuardClauses;

namespace Shopizy.Application.Common.Wrappers;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public bool Succeeded { get; set; }

    public string[] Errors { get; set; }

    public static Result Success() => new(true, Array.Empty<string>());

    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}

public class Success<T>(T results) : IResult<T>
{
    private readonly T _results = results;
    private readonly bool _succeeded = true;

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<GenericResponse, TResult> onError
    )
    {
        Guard.Against.Null(onSuccess);
        return onSuccess(_results);
    }

    public IResult<TResult> Map<TResult>(Func<T, TResult> f)
    {
        Guard.Against.Null(f);
        return new Success<TResult>(f(_results));
    }

    public IResult<TResult> Bind<TResult>(Func<T, IResult<TResult>> f)
    {
        Guard.Against.Null(f);
        return f(_results);
    }

    public bool Succeeded() => _succeeded;
}

public class Error<T>(GenericResponse error) : IResult<T>
{
    private readonly GenericResponse _error = error;
    private readonly bool _succeeded = false;

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<GenericResponse, TResult> onError
    )
    {
        Guard.Against.Null(onError);
        return onError(_error);
    }

    public IResult<TResult> Map<TResult>(Func<T, TResult> f) => new Error<TResult>(_error);

    public IResult<TResult> Bind<TResult>(Func<T, IResult<TResult>> f) =>
        new Error<TResult>(_error);

    public bool Succeeded() => _succeeded;
}
