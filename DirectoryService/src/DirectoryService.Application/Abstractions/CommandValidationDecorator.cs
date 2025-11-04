using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Domain.Shared.Errors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application.Abstractions;

public class CommandValidationDecorator<TResponse, TCommand> : ICommandHandler<TResponse, TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TResponse, TCommand> _commandHandler;
    private readonly IValidator<TCommand>? _validator;

    public CommandValidationDecorator(
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
            if (result.IsValid == false)
            {
                return result.ToErrorList();
            }
        }

        return await _commandHandler.Handle(command, cancellationToken);
    }
}