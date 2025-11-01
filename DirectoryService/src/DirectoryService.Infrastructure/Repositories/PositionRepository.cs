using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;

namespace DirectoryService.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public PositionRepository(DirectoryServiceDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result<Guid, Error>> CreatePosition(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Positions.AddAsync(position, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return Errors.General.SaveFailed(position.Id);
        }

        return position.Id;
    }
}