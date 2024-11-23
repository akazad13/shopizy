namespace Shopizy.Contracts.Common;

public class SuccessResult
{
    public string Message { get; set; }

    internal SuccessResult(string message)
    {
        Message = message;
    }

    public static SuccessResult Success(string message) => new(message);
}
