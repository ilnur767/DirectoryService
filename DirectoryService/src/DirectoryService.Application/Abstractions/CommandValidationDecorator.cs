using CSharpFunctionalExtensions;
using DirectoryService.Application.Extensions;
using DirectoryService.Domain.Shared.Errors;
using FluentValidation;

namespace DirectoryService.Application.Abstractions;

public class CommandValidationDecorator<TResponse, TCommand> : ICommandHandler<TResponse, TCommand>
    where TCommand : ICommand
{
    private readonly ICommandHandler<TResponse, TCommand> _commandHandler;
    private readonly IValidator<TCommand> _validator;

    public CommandValidationDecorator(
        IValidator<TCommand> validator,
        ICommandHandler<TResponse, TCommand> commandHandler)
    {
        _validator = validator;
        _commandHandler = commandHandler;
    }

    public async Task<Result<TResponse, ErrorList>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(command, cancellationToken);
        if (result.IsValid == false)
        {
            return result.ToErrorList();
        }

        return await _commandHandler.Handle(command, cancellationToken);
    }
}