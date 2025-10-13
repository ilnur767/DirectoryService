using CSharpFunctionalExtensions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Application.Abstractions;

public interface ILocationRepository
{
    Task<Result<Guid, Error>> CreateLocation(Location location, CancellationToken cancellationToken);
}