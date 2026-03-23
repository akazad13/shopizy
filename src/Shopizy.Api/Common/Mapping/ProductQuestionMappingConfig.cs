using Mapster;
using Shopizy.Application.ProductQuestions.Commands.AnswerQuestion;
using Shopizy.Application.ProductQuestions.Commands.AskQuestion;
using Shopizy.Contracts.ProductQuestion;
using Shopizy.Domain.ProductQuestions;

namespace Shopizy.Api.Common.Mapping;

public class ProductQuestionMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        config
            .NewConfig<(AskQuestionRequest request, Guid UserId, Guid ProductId), AskQuestionCommand>()
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.Question, src => src.request.Question);

        config
            .NewConfig<(AnswerQuestionRequest request, Guid QuestionId, Guid AnsweredByUserId), AnswerQuestionCommand>()
            .Map(dest => dest.QuestionId, src => src.QuestionId)
            .Map(dest => dest.AnsweredByUserId, src => src.AnsweredByUserId)
            .Map(dest => dest.Answer, src => src.request.Answer);

        config
            .NewConfig<ProductQuestion, ProductQuestionResponse>()
            .Map(dest => dest.QuestionId, src => src.Id.Value)
            .Map(dest => dest.Question, src => src.Question)
            .Map(dest => dest.IsAnswered, src => src.IsAnswered)
            .Map(dest => dest.Answer, src => src.Answer != null ? src.Answer.Answer : null)
            .Map(dest => dest.CreatedOn, src => src.CreatedOn);
    }
}
