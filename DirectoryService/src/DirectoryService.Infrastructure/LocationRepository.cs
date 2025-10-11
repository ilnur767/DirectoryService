using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Infrastructure;

public class LocationRepository : ILocationRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public LocationRepository(DirectoryServiceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Guid, Error>> CreateLocation(Location location, CancellationToken cancelationToken)
    {
        try
        {
            await _dbContext.AddAsync(location, cancelationToken);

            await _dbContext.SaveChangesAsync(cancelationToken);
        }
        catch (Exception e)
        {
            return Errors.General.SaveFailed(location.Id);
        }

        return location.Id;
    }
}