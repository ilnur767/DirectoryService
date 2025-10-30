using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Domain.Shared.Errors;
using FluentValidation;

namespace DirectoryService.Application.Abstractions;

public class QueryValidationDecorator<TResponse, TQuery> : IQueryHandler<TResponse, TQuery>
    where TQuery : IQuery
{
    private readonly IQueryHandler<TResponse, TQuery> _queryHandler;
    private readonly IValidator<TQuery> _validator;

    public QueryValidationDecorator(
        IValidator<TQuery> validator,
        IQueryHandler<TResponse, TQuery> queryHandler)
    {
        _validator = validator;
        _queryHandler = queryHandler;
    }

    public async Task<Result<TResponse, ErrorList>> Handle(TQuery command, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(command, cancellationToken);
        if (result.IsValid == false)
        {
            return result.ToErrorList();
        }

        return await _queryHandler.Handle(command, cancellationToken);
    }
}