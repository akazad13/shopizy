namespace Shopizy.Contracts.Common;

public class ErrorResult
{
    public string Message { get; set; }

    public string[] Errors { get; set; }

    internal ErrorResult(IEnumerable<string> errors)
    {
        Message = "Error occured";
        Errors = errors.ToArray();
    }

    public static ErrorResult Failure(IEnumerable<string> errors) => new(errors);
}
