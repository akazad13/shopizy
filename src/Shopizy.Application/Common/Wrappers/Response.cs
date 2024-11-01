namespace Shopizy.Application.Common.Wrappers;

public class GenericResponse(string message, IEnumerable<string> errors)
{
    public string Message { get; set; } = message;
    public IEnumerable<string> Errors { get; set; } = errors;
}

public static class Response<T>
{
    public static IResult<T> SuccessResponese(T value) => new Success<T>(value);

    public static IResult<GenericResponse> SuccessResponese(string message) =>
        new Success<GenericResponse>(new GenericResponse(message, Array.Empty<string>()));

    public static IResult<T> ErrorResponse(IEnumerable<string> errors) =>
        new Error<T>(new GenericResponse("Error Occured", errors));

    public static IResult<T> ErrorResponse(GenericResponse genericResponse) =>
        new Error<T>(genericResponse);
}
