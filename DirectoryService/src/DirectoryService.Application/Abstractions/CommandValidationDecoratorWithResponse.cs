using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Domain.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application.Abstractions;

public class CommandValidationDecoratorWithResponse<TResponse, TCommand> : ICommandHandler<TResponse, TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TResponse, TCommand> _commandHandler;
    private readonly IValidator<TCommand>? _validator;

    public CommandValidationDecoratorWithResponse(
        IServiceProvider serviceProvider,
        ICommandHandler<TResponse, TCommand> commandHandler)
    {
        _validator = serviceProvider.GetService<IValidator<TCommand>>();
        _commandHandler = commandHandler;
    }

    public async Task<Result<TResponse, ErrorList>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (_validator != null)
        {
            var result = await _validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return result.ToErrorList();
            }
        }

        return await _commandHandler.Handle(command, cancellationToken);
    }
}

public class CommandValidationDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _commandHandler;
    private readonly IValidator<TCommand>? _validator;

    public CommandValidationDecorator(
        IServiceProvider serviceProvider,
        ICommandHandler<TCommand> commandHandler)
    {
        _validator = serviceProvider.GetService<IValidator<TCommand>>();
        _commandHandler = commandHandler;
    }

    public async Task<UnitResult<ErrorList>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (_validator != null)
        {
            var result = await _validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                return result.ToErrorList();
            }
        }

        return await _commandHandler.Handle(command, cancellationToken);
    }
}