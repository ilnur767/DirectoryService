using DirectoryService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

public abstract class TestBase : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected TestBase(DirectoryTestWebFactory factory)
    {
        _resetDatabase = factory.ResetDatabaseAsync;
        Services = factory.Services;
    }

    protected IServiceProvider Services { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    protected async Task<T> ExecuteInDb<T>(Func<DirectoryServiceDbContext, Task<T>> func)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        return await func(dbContext);
    }

    protected async Task ExecuteInDb(Func<DirectoryServiceDbContext, Task> func)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        await func(dbContext);
    }
}