using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Domain.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application.Abstractions;

public class QueryValidationDecorator<TResponse, TQuery> : IQueryHandler<TResponse, TQuery>
    where TQuery : IQuery
{
    private readonly IQueryHandler<TResponse, TQuery> _queryHandler;
    private readonly IValidator<TQuery>? _validator;

    public QueryValidationDecorator(
        IServiceProvider serviceProvider,
        IQueryHandler<TResponse, TQuery> queryHandler)
    {
        _validator = serviceProvider.GetService<IValidator<TQuery>>();
        _queryHandler = queryHandler;
    }

    public async Task<Result<TResponse, ErrorList>> Handle(TQuery command, CancellationToken cancellationToken)
    {
        if (_validator != null)
        {
            var result = await _validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return result.ToErrorList();
            }
        }

        return await _queryHandler.Handle(command, cancellationToken);
    }
}