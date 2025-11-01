using CSharpFunctionalExtensions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Abstractions;

public interface IPositionRepository
{
    Task<Result<Guid, Error>> CreatePosition(Position department, CancellationToken cancellationToken);
}