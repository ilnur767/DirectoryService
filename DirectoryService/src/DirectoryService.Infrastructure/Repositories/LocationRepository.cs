using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public LocationRepository(DirectoryServiceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Guid, Error>> CreateLocation(Location location, CancellationToken cancelationToken)
    {
        try
        {
            await _dbContext.Locations.AddAsync(location, cancelationToken);

            await _dbContext.SaveChangesAsync(cancelationToken);
        }
        catch (Exception e)
        {
            return Errors.General.SaveFailed(location.Id);
        }

        return location.Id;
    }

    public async Task<Result<bool, Error>> CheckIfAllLocationsExist(Guid[] ids, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dbContext.Locations
                .Where(l => ids.Contains(l.Id) && l.IsActive == true)
                .CountAsync(cancellationToken);

            if (result != ids.Length)
            {
                return Errors.General.NotFound("Not all locations exists");
            }

            return true;
        }
        catch (Exception e)
        {
            return Errors.General.GetFailed();
        }
    }
}