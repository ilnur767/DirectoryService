using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Database;

public class TransactionScope : ITransactionScope
{
    private readonly IDbTransaction _dbTransaction;
    private readonly ILogger<TransactionScope> _logger;

    public TransactionScope(IDbTransaction transaction, ILogger<TransactionScope> logger)
    {
        _dbTransaction = transaction;
        _logger = logger;
    }

    public UnitResult<Error> Commit()
    {
        try
        {
            _dbTransaction.Commit();

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit transaction");

            return Error.Failure("transaction.commit.failed", "Failed to commit transaction");
        }
    }

    public UnitResult<Error> Rollback()
    {
        try
        {
            _dbTransaction.Rollback();

            return UnitResult.Success<Error>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback transaction");

            return Error.Failure("transaction.rollback.failed", "Failed to rollback transaction");
        }
    }

    public void Dispose() => _dbTransaction.Dispose();
}