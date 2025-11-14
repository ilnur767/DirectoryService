using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DirectoryService.Infrastructure.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<DepartmentRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> CreateDepartment(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.AddAsync(department, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return Errors.General.SaveFailed(department.Id);
        }

        return department.Id;
    }

    public async Task<Result<Department, Error>> GetDepartmentById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _dbContext.Departments
                    .Include(d => d.DepartmentLocations)
                    .Include(d => d.Parent)
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsActive == true,
                        cancellationToken);

            if (result == null)
            {
                return Errors.General.NotFound(id);
            }

            return result;
        }
        catch (Exception e)
        {
            return Errors.General.GetFailed(id);
        }
    }

    public async Task<Result<bool, Error>> CheckIfAllDepartmentsExist(Guid[] ids, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dbContext.Departments
                .Where(l => ids.Contains(l.Id) && l.IsActive == true)
                .CountAsync(cancellationToken);

            if (result != ids.Length)
            {
                return Errors.General.NotFound("Not all departments exists");
            }

            return true;
        }
        catch (Exception e)
        {
            return Errors.General.GetFailed();
        }
    }

    public async Task<UnitResult<Error>> Save(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Saving not completed");

            return Errors.General.SaveFailed();
        }

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> UpdateChildrenPathAndDepth(Department department,
        DepartmentPath oldPath,
        DateTime updateAt,
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = """
                               UPDATE departments 
                               SET 
                                    depth =  nlevel(@path::ltree || subpath(path, nlevel(@oldPath::ltree))) - 1,
                                    path = @path || subpath(path, nlevel(@oldPath::ltree)),
                                    updated_at = @updateAt
                               WHERE id <> @id AND path <@  @oldPath::ltree
                               """;

            var parameters = new[]
            {
                new NpgsqlParameter("oldPath", oldPath.Value), new NpgsqlParameter("path", department.Path.Value),
                new NpgsqlParameter("id", department.Id), new NpgsqlParameter("updateAt", updateAt)
            };

            await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Updating children path and depth not successful");
            return Errors.General.Failure();
        }
    }

    public async Task<Result<bool, Error>> IsDescendant(Guid departmentId, Guid departmentIdForCheck,
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = """
                               SELECT 1
                               FROM departments d
                               WHERE path <@ (SELECT path FROM departments d WHERE d.id = @departmentId)::ltree
                                    AND id =@departmentIdForCheck
                               """;

            var parameters = new[]
            {
                new NpgsqlParameter("departmentId", departmentId),
                new NpgsqlParameter("departmentIdForCheck", departmentIdForCheck)
            };

            var result = await _dbContext.Departments
                .FromSqlRaw(
                    sql,
                    parameters)
                .AnyAsync(cancellationToken);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not check department for descendants");
            return Errors.General.Failure();
        }
    }

    public async Task<UnitResult<Error>> LockDescendantsRecursive(Guid departmentId,
        CancellationToken cancellationToken)
    {
        try
        {
            const string sql = @"
                    WITH RECURSIVE descendants AS (
                        SELECT d.id
                        FROM departments d
                        WHERE d.id = @departmentId

                        UNION ALL

                        SELECT c.id
                        FROM departments c
                        JOIN descendants p ON c.parent_id = p.id
                    )
                    SELECT *
                    FROM departments d
                    JOIN descendants ds ON d.id = ds.id
                    WHERE d.is_active=true
                    FOR UPDATE;";

            var param = new NpgsqlParameter("departmentId", departmentId);

            await _dbContext.Database.ExecuteSqlRawAsync(sql, [param], cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Locking descendants not successful");
            return Errors.General.Failure();
        }
    }

    public async Task<UnitResult<Error>> LockDescendantsByLtree(Guid departmentId, CancellationToken cancellationToken)
    {
        try
        {
            const string sql = @"
                    SELECT *
                    FROM departments d
                    WHERE path <@ (SELECT path FROM departments d WHERE d.id = @departmentId)::ltree
                    FOR UPDATE;";

            var param = new NpgsqlParameter("departmentId", departmentId);

            await _dbContext.Database.ExecuteSqlRawAsync(sql, [param], cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Locking descendants not successful");
            return Errors.General.Failure();
        }
    }
}