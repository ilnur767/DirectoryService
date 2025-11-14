using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Database;

public class TransactionManager : ITransactionManager
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DirectoryServiceDbContext> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public TransactionManager(DirectoryServiceDbContext dbContext, ILogger<DirectoryServiceDbContext> logger,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<Result<ITransactionScope, Error>> BeginTransaction(CancellationToken cancellationToken = default,
        IsolationLevel? isolationLevel = null)
    {
        try
        {
            var transaction =
                await _dbContext.Database.BeginTransactionAsync(isolationLevel ?? IsolationLevel.ReadCommitted,
                    cancellationToken);

            var transactionScopeLogger = _loggerFactory.CreateLogger<TransactionScope>();

            var transactionScope = new TransactionScope(transaction.GetDbTransaction(), transactionScopeLogger);

            return transactionScope;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to begin transaction");

            return Error.Failure("database", "Failed to begin transaction");
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to save changes");

            return Error.Failure("database", "Failed to save changes");
        }

        return UnitResult.Success<Error>();
    }
}