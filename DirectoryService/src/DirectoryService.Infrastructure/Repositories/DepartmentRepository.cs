using CSharpFunctionalExtensions;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.Enities;
using DirectoryService.Domain.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Repositories;

public sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public DepartmentRepository(DirectoryServiceDbContext dbContext) => _dbContext = dbContext;

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
                await _dbContext.Departments.FirstOrDefaultAsync(d => d.Id == id && d.IsActive == true,
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
}