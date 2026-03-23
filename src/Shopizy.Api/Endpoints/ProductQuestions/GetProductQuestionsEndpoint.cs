using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.Application.ProductQuestions.Queries.GetProductQuestions;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.ProductQuestion;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Api.Endpoints.ProductQuestions;

public class GetProductQuestionsEndpoint : ApiEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "api/v1.0/products/{productId:guid}/questions",
            async (
                Guid productId,
                [FromServices] IDispatcher mediator,
                IMapper mapper,
                ILogger<GetProductQuestionsEndpoint> logger
            ) =>
            {
                return await HandleAsync(
                    mediator,
                    new GetProductQuestionsQuery(productId),
                    questions => Results.Ok(mapper.Map<IReadOnlyList<ProductQuestionResponse>>(questions)),
                    ex => logger.ProductQuestionFetchError(ex)
                );
            }
        )
        .AllowAnonymous()
        .WithTags("ProductQuestions")
        .WithSummary("Get product questions")
        .WithDescription("Returns all questions and answers for a product.")
        .Produces<IReadOnlyList<ProductQuestionResponse>>(StatusCodes.Status200OK)
        .Produces<ErrorResult>(StatusCodes.Status500InternalServerError);
    }
}
