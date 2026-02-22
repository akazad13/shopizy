using ErrorOr;
using FluentValidation;
using MediatR;

namespace Shopizy.SharedKernel.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (_validator is null)
        {
#pragma warning disable CA2016
            return await next();
#pragma warning restore CA2016
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
#pragma warning disable CA2016
            return await next();
#pragma warning restore CA2016
        }

        var errors = validationResult.Errors.ConvertAll(error =>
            Error.Validation(code: error.PropertyName, description: error.ErrorMessage)
        );

        return (dynamic)errors;
    }
}
