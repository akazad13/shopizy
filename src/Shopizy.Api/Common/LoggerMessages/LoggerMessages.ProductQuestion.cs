namespace Shopizy.Api.Common.LoggerMessages;

public static partial class LoggerMessages
{
    [LoggerMessage(EventId = 1047, Level = LogLevel.Error, Message = "An error occurred while fetching product questions.")]
    public static partial void ProductQuestionFetchError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1048, Level = LogLevel.Error, Message = "An error occurred while creating product question.")]
    public static partial void ProductQuestionCreationError(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 1049, Level = LogLevel.Error, Message = "An error occurred while answering product question.")]
    public static partial void ProductQuestionAnswerError(this ILogger logger, Exception ex);
}
