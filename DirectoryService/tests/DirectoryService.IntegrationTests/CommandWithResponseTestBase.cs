using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public abstract class CommandWithResponseTestBase<TResponse, TCommand> : TestBase
    where TCommand : ICommand
{
    public CommandWithResponseTestBase(DirectoryTestWebFactory factory) : base(factory)
    {
    }

    protected async Task<Result<TResponse, ErrorList>> HandleCommand(TCommand command)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<ICommandHandler<TResponse, TCommand>>();

        return await sut.Handle(command, CancellationToken.None);
    }
}

public abstract class CommandTestBase<TCommand> : TestBase
    where TCommand : ICommand
{
    public CommandTestBase(DirectoryTestWebFactory factory) : base(factory)
    {
    }

    protected async Task<UnitResult<ErrorList>> HandleCommand(TCommand command)
    {
        await using var scope = Services.CreateAsyncScope();
        var sut = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        return await sut.Handle(command, CancellationToken.None);
    }
}