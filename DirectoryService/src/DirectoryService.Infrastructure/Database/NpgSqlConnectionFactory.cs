using System.Data;
using DirectoryService.Application.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Database;

public class NpgSqlConnectionFactory : IDbConnectionFactory, IDisposable, IAsyncDisposable
{
    public NpgsqlDataSource _dataSource;

    public NpgSqlConnectionFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        var builder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("DirectoryServiceDb"));

        builder.UseLoggerFactory(loggerFactory);
        builder.EnableParameterLogging();
        _dataSource = builder.Build();
    }

    public ValueTask DisposeAsync() => _dataSource.DisposeAsync();

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return connection;
    }

    public void Dispose() => _dataSource.Dispose();
}