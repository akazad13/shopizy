using ErrorOr;
using FluentValidation;
using MediatR;

namespace Shopizy.Application.Common.Behaviors;

// public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
//     : IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IRequest<TResponse>
// {
//     private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

//     public async Task<TResponse> Handle(
//         TRequest request,
//         RequestHandlerDelegate<TResponse> next,
//         CancellationToken cancellationToken
//     )
//     {
//         if (_validators.Any())
//         {
//             var context = new ValidationContext<TRequest>(request);

//             var validationResults = await Task.WhenAll(
//                 _validators.Select(v => v.ValidateAsync(context, cancellationToken))
//             );

//             var failures = validationResults
//                 .Where(r => r.Errors.Count != 0)
//                 .SelectMany(r => r.Errors)
//                 .ToList();

//             if (failures.Count != 0)
//             {
//                 throw new ValidationException(failures);
//             }
//         }
//         return await next();
//     }
// }

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
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();
        }

        var errors = validationResult.Errors.ConvertAll(error =>
            Error.Validation(code: error.PropertyName, description: error.ErrorMessage)
        );

        return (dynamic)errors;
    }
}
