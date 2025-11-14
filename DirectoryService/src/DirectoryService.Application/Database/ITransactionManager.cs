using System.Data;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Database;

public interface ITransactionManager
{
    Task<Result<ITransactionScope, Error>> BeginTransaction(CancellationToken cancellationToken = default,
        IsolationLevel? isolationLevel = null);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}