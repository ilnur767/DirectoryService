using System.Data.Common;
using DirectoryService.Infrastructure;
using DirectoryService.Presentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace DirectoryService.IntegrationTests;

public class DirectoryTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("postgres") // Служебная база для возможности удаления/создания других баз
        .WithDatabase("directory_service") // Основная тестовая база приложения
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithImage("postgres:16.8")
        .Build();

    private DbConnection _dbConnection;

    private Respawner _respawner;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _dbConnection.OpenAsync();
        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await ResetDatabaseAsync();
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
        await _dbConnection.CloseAsync();
        await _dbConnection.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureAppConfiguration((context, configBuidler) =>
        {
            configBuidler.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DirectoryServiceDb"] = _dbContainer.GetConnectionString()
            });
        });

    private async Task InitializeRespawner() =>
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["public"] });

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_dbConnection);
}