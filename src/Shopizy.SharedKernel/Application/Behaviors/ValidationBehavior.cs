using ErrorOr;
using FluentValidation;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.SharedKernel.Application.Behaviors;

public class ValidationCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IValidator<TCommand>? validator = null)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : IErrorOr
{
    private readonly ICommandHandler<TCommand, TResponse> _innerHandler = innerHandler;
    private readonly IValidator<TCommand>? _validator = validator;

    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        if (_validator is null)
        {
            return await _innerHandler.Handle(command, cancellationToken);
        }

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (validationResult.IsValid)
        {
            return await _innerHandler.Handle(command, cancellationToken);
        }

        var errors = validationResult.Errors.ConvertAll(error =>
            Error.Validation(code: error.PropertyName, description: error.ErrorMessage)
        );

        return (dynamic)errors;
    }
}

public class ValidationQueryHandlerDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> innerHandler,
    IValidator<TQuery>? validator = null)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : IErrorOr
{
    private readonly IQueryHandler<TQuery, TResponse> _innerHandler = innerHandler;
    private readonly IValidator<TQuery>? _validator = validator;

    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default)
    {
        if (_validator is null)
        {
            return await _innerHandler.Handle(query, cancellationToken);
        }

        var validationResult = await _validator.ValidateAsync(query, cancellationToken);

        if (validationResult.IsValid)
        {
            return await _innerHandler.Handle(query, cancellationToken);
        }

        var errors = validationResult.Errors.ConvertAll(error =>
            Error.Validation(code: error.PropertyName, description: error.ErrorMessage)
        );

        return (dynamic)errors;
    }
}
