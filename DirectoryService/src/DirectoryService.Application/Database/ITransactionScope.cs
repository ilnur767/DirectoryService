using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();
    UnitResult<Error> Rollback();
}